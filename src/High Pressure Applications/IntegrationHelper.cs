using System.Collections.Generic;
using STRINGS;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using High_Pressure_Applications.Components;

namespace High_Pressure_Applications
{
    public static class Integration
    {
        private static readonly FieldInfo maxMass = AccessTools.Field(typeof(ConduitFlow), "MaxMass");
        private static readonly FieldInfo conduitType = AccessTools.Field(typeof(ConduitFlow), "conduitType");
        private static readonly MethodBase patch = AccessTools.Method(typeof(Integration), nameof(IntegratePressurized));
        private static readonly MethodBase overpressurePatch = AccessTools.Method(typeof(Integration), nameof(IntegrateOverpressure));
        private static readonly MethodBase conduitContentsAddMass = AccessTools.Method(typeof(ConduitFlow.ConduitContents), "AddMass");

        //Replace references of ConduitFlow.MaxMass with a custom handler to determine if the MaxMass should be higher for pressurized pipes
        internal static IEnumerable<CodeInstruction> AddIntegrationIfNeeded(CodeInstruction original, CodeInstruction toGetCell, bool isUpdateConduit = false)
        {
            //If the load field operand is being used to retrieve the maxMass field, override the maxMass value with our own max mass if necessary.
            //For example, if looking at a high pressure gas pipe, max mass will return 1000, even though we want the max mass of the high pressure pipe to be 3000.
            //The "toGetCell" code instruction will load the variable containing the cell index we need to look at
            if (original.opcode == OpCodes.Ldfld && (original.operand as FieldInfo) == maxMass)
            {
                yield return original;
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, conduitType);
                yield return toGetCell;
                yield return new CodeInstruction(OpCodes.Call, patch);

            }
            //During the UpdateConduit method, the ConduitContents.AddMass method is called to move the contents from one pipe to the next.
            //In order to integrate an overpressure functionality, hook on just after the masses are added, and determine if overpressure damage needs to be dealt to the receiving conduit.
            else if (isUpdateConduit && original.opcode == OpCodes.Call && (original.operand as MethodBase) == conduitContentsAddMass)
            {
                yield return original;
                yield return new CodeInstruction(OpCodes.Ldloc_2); //gridenode grid_node
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, maxMass); //this.MaxMass
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, conduitType); //this.conduitType
                yield return toGetCell; //int cell2
                yield return new CodeInstruction(OpCodes.Call, overpressurePatch);
            }
            else
                yield return original;
        }
        internal static int[] layers = { 0, 12, 16, 0 };
        internal static int[] connectionLayers = { 0, 15, 19, 0 };

       //Based on the passed variables, determine if overpressure damage should be dealt to the receiving conduit
        private static void IntegrateOverpressure(ConduitFlow.GridNode sender, float standardMax, ConduitType conduitType, int cell)
        {
            GameObject receiver;
            float receiverMax = Integration.GetMaxCapacityWithObject(cell, conduitType, out receiver);
            float senderMass = sender.contents.mass;

            //33% chance to damage the receiver when sender has double its capacity if the receiver is not a bridge
            //if receiver is a bridge, 33% to damage the bridge if the sender's contents are above the bridge's capacity at all
            if (senderMass >= receiverMax * 2f  && Random.Range(0f, 1f) < 0.33f)
            {
                //This damage CANNOT be dealt immediately, or it will cause the game to crash. This code execution occurs during an UpdateNetworkTask execution and does not seem to support executing Triggers
                //The damage will instead be queued and dealt during the next Game.Update (the next tick of the game)
                queueDamages.Add(new QueueDamage(receiver));
            }
        }

        //Clear these variables when loading a save, otherwise old data may persist between loads
        public static void ClearStaticInfo()
        {
            QueueDamage.ClearLastNotification();
            queueDamages = new List<QueueDamage>();
        }
        //Get a DamageSourceInfo object for damage being dealth from overpressure
        public static BuildingHP.DamageSourceInfo GetPressureDamage()
        {
            BuildingHP.DamageSourceInfo damage = new BuildingHP.DamageSourceInfo
            {
                damage = 1,
                source = BUILDINGS.DAMAGESOURCES.LIQUID_PRESSURE,
                popString = STRINGS.UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LIQUID_PRESSURE
            };
            return damage;
        }

        public static void DoPressureDamage(GameObject obj)
        {
            BuildingHP.DamageSourceInfo damage = GetPressureDamage();
            obj.Trigger((int)GameHashes.DoBuildingDamage, damage);
            QueueDamage.ExecuteNotification(obj);
        }
        //To store the damage information until it is able to be applied
        internal class QueueDamage
        {
            public GameObject Receiver;
            private static Notification lastNotification = null;
            public QueueDamage(GameObject rcvr)
            {
                Receiver = rcvr;
            }

            public static void ExecuteNotification(GameObject obj)
            {
                Notification result = new Notification("Pipe Overpressure", NotificationType.BadMinor, null, null, true, 0f, null, null, obj.transform, true);
                Notifier notifier = Game.Instance.FindOrAdd<Notifier>();
                if (lastNotification != null)
                {
                    notifier.Remove(lastNotification);
                    lastNotification = null;
                }

                notifier.Add(result);
                lastNotification = result;
            }
            internal static void ClearLastNotification()
            {
                lastNotification = null;
            }
        }

        internal static List<QueueDamage> queueDamages = new List<QueueDamage>();

        //If the specified conduit is not a default conduit, replace the standardMax with the conduits actual max capacity
        private static float IntegratePressurized(float standardMax, ConduitType conduitType, int cell)
        {
            return GetMaxCapacityAt(cell, conduitType);
        }

        public static GameObject GetConduitObjectAt(int cell, ConduitType type, bool isBridge = false)
        {
            if (type != ConduitType.Gas && type != ConduitType.Liquid)
                return null;
            int layer = isBridge ? connectionLayers[(int)type] : layers[(int)type];
            return Grid.Objects[cell, layer];
        }

        //Utility function, given the cell, conduit type, and whether or not the conduit is a bridge, attempt to find the conduit at that location and retrieve its Pressurized component
        public static Pressurized GetPressurizedAt(int cell, ConduitType type, bool isBridge = false)
        {
            return GetConduitObjectAt(cell, type, isBridge)?.GetComponent<Pressurized>();
        }

        public static float GetMaxCapacityAt(int cell, ConduitType type, bool isBridge = false)
        {

            if (type != ConduitType.Gas && type != ConduitType.Liquid)
                throw new System.ArgumentException($"[Pressurized] Invalid Conduit Type given to IntegrationHelper.GetMaxCapacityAt(): {type.ToString()}  Type must be ConduitType.Gas or ConduitType.Liquid.", "type");
            ConduitFlow manager = Conduit.GetFlowManager(type);
            return Pressurized.GetMaxCapacity(GetPressurizedAt(cell, type, isBridge));
        }

        public static float GetMaxCapacityWithObject(int cell, ConduitType type, out GameObject obj, bool isBridge = false)
        {
            if (type != ConduitType.Gas && type != ConduitType.Liquid)
                throw new System.ArgumentException($"[Pressurized] Invalid Conduit Type given to IntegrationHelper.GetMaxCapacityAt(): {type.ToString()}  Type must be ConduitType.Gas or ConduitType.Liquid.", "type");
            ConduitFlow manager = Conduit.GetFlowManager(type);
            Pressurized pressurized = GetPressurizedAt(cell, type, isBridge);
            if (pressurized == null)
                obj = null;
            else
                obj = pressurized.gameObject;
            if (Pressurized.IsDefault(pressurized))
                return manager.MaxMass();
            else
                return pressurized.Info.IncreaseMultiplier * manager.MaxMass();
        }

    }
}
