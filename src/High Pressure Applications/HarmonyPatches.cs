using HarmonyLib;
using High_Pressure_Applications.BuildingConfigs;
using High_Pressure_Applications.Components;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace High_Pressure_Applications
{
    internal static partial class HarmonyPatches
    {
        //MaxMass is used by:
        //ConduitFlow.UpdateConduit
        //ConduitFlow.AddElement
        //ConduitFlow.OnDeserialized
        //ConduitFlow.IsConduitFull
        //ConduitFlow.FreezeConduitContents
        //ConduitFlow.MeltConduitContents
        private static readonly FieldInfo maxMass = AccessTools.Field(typeof(ConduitFlow), "MaxMass");

        //Add the new buildings to the database and building plan screen
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
        public static class HighPressure_GeneratedBuildings_LoadGeneratedBuildings
        {
            public static void Prefix()
            {
                //PRESSURIZED GAS PIPE
                string prefix = "STRINGS.BUILDINGS.PREFABS." + HighPressureGasConduitConfig.Id.ToUpper();
                Strings.Add(prefix + ".NAME", HighPressureGasConduitConfig.DisplayName);
                Strings.Add(prefix + ".DESC", HighPressureGasConduitConfig.Description);
                Strings.Add(prefix + ".EFFECT", HighPressureGasConduitConfig.Effect);
                ModUtil.AddBuildingToPlanScreen("HVAC", HighPressureGasConduitConfig.Id);

                //PRESSURIZED GAS BRIDGE
                prefix = "STRINGS.BUILDINGS.PREFABS." + HighPressureGasConduitBridgeConfig.Id.ToUpper();
                Strings.Add(prefix + ".NAME", HighPressureGasConduitBridgeConfig.DisplayName);
                Strings.Add(prefix + ".DESC", HighPressureGasConduitBridgeConfig.Description);
                Strings.Add(prefix + ".EFFECT", HighPressureGasConduitBridgeConfig.Effect);
                ModUtil.AddBuildingToPlanScreen("HVAC", HighPressureGasConduitBridgeConfig.Id);

                //PRESSURIZED LIQUID PIPE
                prefix = "STRINGS.BUILDINGS.PREFABS." + HighPressureLiquidConduitConfig.Id.ToUpper();
                Strings.Add(prefix + ".NAME", HighPressureLiquidConduitConfig.DisplayName);
                Strings.Add(prefix + ".DESC", HighPressureLiquidConduitConfig.Description);
                Strings.Add(prefix + ".EFFECT", HighPressureLiquidConduitConfig.Effect);
                ModUtil.AddBuildingToPlanScreen("Plumbing", HighPressureLiquidConduitConfig.Id);

                //PRESSURIZED LIQUID BRIDGE
                prefix = "STRINGS.BUILDINGS.PREFABS." + HighPressureLiquidConduitBridgeConfig.Id.ToUpper();
                Strings.Add(prefix + ".NAME", HighPressureLiquidConduitBridgeConfig.DisplayName);
                Strings.Add(prefix + ".DESC", HighPressureLiquidConduitBridgeConfig.Description);
                Strings.Add(prefix + ".EFFECT", HighPressureLiquidConduitBridgeConfig.Effect);
                ModUtil.AddBuildingToPlanScreen("Plumbing", HighPressureLiquidConduitBridgeConfig.Id);
            }
        }


        //Place the new buildings under appropriate tech groupings (research trees)
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class HighPressure_Db_Initialize
        {
            private static void Postfix()
            {
                Db.Get().Techs.Get("HVAC").unlockedItemIDs.Add(HighPressureGasConduitConfig.Id);
                Db.Get().Techs.Get("HVAC").unlockedItemIDs.Add(HighPressureGasConduitBridgeConfig.Id);

                Db.Get().Techs.Get("LiquidTemperature").unlockedItemIDs.Add(HighPressureLiquidConduitConfig.Id);
                Db.Get().Techs.Get("LiquidTemperature").unlockedItemIDs.Add(HighPressureLiquidConduitBridgeConfig.Id);
            }
        }


        //Add the HighPressure component to every ConduitBridge during Prefab Initialization
        [HarmonyPatch(typeof(ConduitBridge), "OnPrefabInit")]
        internal static class Patch_ConduitBridge_OnPrefabInit
        {
            internal static void Postfix(ConduitBridge __instance)
            {
                __instance.gameObject.AddOrGet<Pressurized>();
            }
        }

        //Add the HighPressure component to every Conduit during Prefab Initialization
        [HarmonyPatch(typeof(Conduit), "OnPrefabInit")]
        internal static class Patch_Conduit_OnPrefabInit
        {
            internal static void Postfix(Conduit __instance)
            {
                __instance.gameObject.AddOrGet<Pressurized>();
            }
        }

        //Cannot trigger building damage inside of the conduit updates (where the need to damage is discovered). Trigger all damage at the end of each tick safely.
        [HarmonyPatch(typeof(Game), "Update")]
        internal static class Patch_Game_Update
        {
            internal static void Postfix()
            {
                List<Integration.QueueDamage> damages = Integration.queueDamages;
                if (damages.Count > 0)
                {
                    foreach (Integration.QueueDamage info in damages)
                        Integration.DoPressureDamage(info.Receiver);
                    damages.Clear();
                }
            }
        }

        [HarmonyPatch(typeof(Game), "OnLoadLevel")]
        internal static class Patch_Game_OnLoad
        {
            internal static void Postfix()
            {
                Integration.ClearStaticInfo();
            }
        }




        //Integrate overpressure damage when a Gas or Liquid Shutoff has too much pressure in the receiving pipe for the output pipe to handle.
        //The shutoff valves have a built-in limit, but has been overriden with a different harmony patch
        //Patch in through ValveBase and check if the instance is of type OperationalValve (the name in the code for shutoffs)
        [HarmonyPatch(typeof(ValveBase), "ConduitUpdate")]
        internal static class Patch_ValveBase_ConduitUpdate
        {
            private static FieldInfo valveBaseOutputCell;

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo conduitGetContents = AccessTools.Method(typeof(ConduitFlow.Conduit), "GetContents");
                MethodInfo overPressurePatch = AccessTools.Method(typeof(Patch_ValveBase_ConduitUpdate), nameof(OperationalValveOverPressure));
                valveBaseOutputCell = AccessTools.Field(typeof(ValveBase), "outputCell");
                foreach (CodeInstruction original in instructions)
                {
                    //Integrate patch when the following line is called:
                    //  ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager)
                    //Utilize the contents value to determine if overpressure damage is necessary
                    if (original.opcode == OpCodes.Call && (original.operand as MethodInfo) == conduitGetContents)
                    {
                        yield return original; //ConduitFlow.ConduitContents contents
                        yield return new CodeInstruction(OpCodes.Ldarg_0); //this (ValveBase)
                        yield return new CodeInstruction(OpCodes.Ldloc_0); //ConduitFlow flowManager
                        yield return new CodeInstruction(OpCodes.Call, overPressurePatch);
                    }
                    else
                        yield return original;
                }
            }

            private static ConduitFlow.ConduitContents OperationalValveOverPressure(ConduitFlow.ConduitContents contents, ValveBase valveBase, ConduitFlow flowManager)
            {
                OperationalValve op;
                if (op = valveBase as OperationalValve)
                {
                    if (op.CurrentFlow > 0f)
                    {
                        int outputCell = (int)valveBaseOutputCell.GetValue(valveBase);
                        GameObject outputObject;
                        float outputCapacity = Integration.GetMaxCapacityWithObject(outputCell, valveBase.conduitType, out outputObject);
                        float inputMass = contents.mass;
                        //If there is greater than 200% of the outputs capacity inside the shutoff valves input pipe, deal overpressure damage 33% of the time.
                        if (inputMass > (outputCapacity * 2) && UnityEngine.Random.Range(0f, 1f) < 0.33f)
                            Integration.DoPressureDamage(outputObject);
                    }
                }
                //since this patch consumed the contents variable on the stack, return the contents back to prevent issues with the next code statement in IL
                return contents;
            }

        }

        //Integrate a max flow rate specifically for ConduitBridges (i.e. Gas Bridge)
        //Normally, a Gas Bridge can move 3KG of gas from one pressurized pipe to another pressurized pipe, since inputs and outputs for buildings have no built in limiter to their flow rate.
        //For ConduitBridges, limit standard bridges to the same standard max flow rate as their respective conduits (1KG for gas bridge and 10KG for liquid bridge).
        [HarmonyPatch(typeof(ConduitBridge), "ConduitUpdate")]
        internal static class Patch_ConduitBridge_ConduitUpdate
        {
            private static FieldInfo bridgeOutputCell;
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo flowManagerGetContents = AccessTools.Method(typeof(ConduitFlow), "GetContents");
                MethodInfo setMaxFlowPatch = AccessTools.Method(typeof(Patch_ConduitBridge_ConduitUpdate), nameof(SetMaxFlow));
                bridgeOutputCell = AccessTools.Field(typeof(ConduitBridge), "outputCell");
                foreach (CodeInstruction original in instructions)
                {
                    if (original.opcode == OpCodes.Callvirt && (original.operand as MethodInfo) == flowManagerGetContents)
                    {
                        yield return original; //flowManager.GetContents(inputCell)
                        yield return new CodeInstruction(OpCodes.Ldarg_0); //this
                        yield return new CodeInstruction(OpCodes.Ldloc_0); //ConduitFlow flowManager
                        yield return new CodeInstruction(OpCodes.Call, setMaxFlowPatch); //SetMaxFlow(flowManager.GetContents
                    }
                    else
                        yield return original;
                }
            }

            private static ConduitFlow.ConduitContents SetMaxFlow(ConduitFlow.ConduitContents contents, ConduitBridge bridge, ConduitFlow manager)
            {
                //If the bridge is broken, prevent the bridge from operating by limiting what it sees.
                if (bridge.GetComponent<BuildingHP>().HitPoints == 0)
                {
                    //does not actually remove mass from the conduit, just causes the bridge to assume there is no mass available to move.
                    contents.RemoveMass(contents.mass);
                    return contents;
                }

                GameObject outputObject;
                int outputCell = (int)bridgeOutputCell.GetValue(bridge);
                float targetCapacity = Integration.GetMaxCapacityWithObject(outputCell, bridge.type, out outputObject, false);
                if (outputObject == null)
                    return contents;
                float capacity = Pressurized.GetMaxCapacity(bridge.GetComponent<Pressurized>());

                //If the ConduitBridge is not supposed to support the amount of fluid currently in the contents, only make the bridge's intended max visible
                //Also immediately deal damage if the current contents are higher than 110% of the intended max (110% is set because at 100%, a system with no pressurized pipes would seem to randomly deal damage as if the contents
                //  were barely over 100%
                if (contents.mass > capacity)
                {
                    if (contents.mass > capacity * 1.1)
                        Integration.DoPressureDamage(bridge.gameObject);

                    float initial = contents.mass;
                    float removed = contents.RemoveMass(initial - capacity);
                    float ratio = removed / initial;
                    contents.diseaseCount = (int)(contents.diseaseCount * ratio);
                }


                if (contents.mass > targetCapacity * 2 && UnityEngine.Random.Range(0f, 1f) < 0.33f)
                {
                    Integration.DoPressureDamage(outputObject);
                }

                return contents;
            }
        }
    }
}
