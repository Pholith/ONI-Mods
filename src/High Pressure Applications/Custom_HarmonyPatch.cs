using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using High_Pressure_Applications.Components;

namespace High_Pressure_Applications
{
    public static class CustomPatcher
    {
        private static MethodInfo Original_Target = AccessTools.Method(RenderMeshTask, "Run", new Type[] { RenderMeshContext });
        private static MethodInfo Patch_Transpiler = typeof(CustomPatcher).GetMethod(nameof(Run_Transpiler));
        private static FieldInfo Context_Outer = AccessTools.Field(RenderMeshContext, "outer");
        private static readonly Type RenderMeshContext = AccessTools.TypeByName("ConduitFlowVisualizer+RenderMeshContext");
        private static Type RenderMeshTask = AccessTools.TypeByName("ConduitFlowVisualizer+RenderMeshTask");
        //The method we want to target is in the class ConduitFlowVisualizer.RenderMeshTask. However, this class is private, and cannot be retrieved by a simple typeof(class) call.
        //Because of this, we cannot use the simplified HarmonyPatch functionality, since the AccessTools.TypeByName cannot be placed in the Attributes of the class (i.e. [Harmony(AccessTools.TypeByName("ConduitFlowVisualier+RenderMeshTask"))] does not work)
        public static void PostPatch(Harmony instance)
        {
            MethodInfo[] methodList = RenderMeshTask.GetMethods();
            foreach (MethodInfo method in methodList)
            {
                if (method.Name == "Run")
                {
                    Original_Target = method;
                }
            }
            //Ensure this field is set inside here, since the static variable may not be able to retrieve the correct value when calculated at the beginning of the program execution
            Context_Outer = AccessTools.Field(RenderMeshContext, "outer");
            //var harmony = Harmony.Create("Super_Corgi_PressurizedPipes_CustomHarmony");
            instance.Patch(Original_Target, null, null, new HarmonyMethod(Patch_Transpiler));
        }

        //Purpose: To modify the size of the flowing contents to accomodate the potential increased capacity of some conduits
        //Normally, the flow caps off in scaling at 1KG/10KG (gas/liquid). Change the behaviour to consider if the contents are in a higher capacity conduit than is standard.
        //Addtionally, if flowing to differing capacities, progressively interlopate the scaling from the source conduit to the scaling of the receiving conduit
        public static IEnumerable<CodeInstruction> Run_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo target = typeof(ConduitFlowVisualizer).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.Name == "CalculateMassScale");
            MethodBase massPatch = AccessTools.Method(typeof(CustomPatcher), nameof(ModifyApparentMass));
            MethodBase lerpPatch = AccessTools.Method(typeof(CustomPatcher), nameof(LerpMasses));
            FieldInfo flowManager = AccessTools.Field(typeof(ConduitFlowVisualizer), "flowManager");
            FieldInfo conduitType = AccessTools.Field(typeof(ConduitFlow), "conduitType");
            FieldInfo conduitContents = AccessTools.Field(typeof(ConduitFlow.ConduitFlowInfo), "contents");
            MethodInfo conduitContentsMass = AccessTools.Property(typeof(ConduitFlow.ConduitContents), "mass").GetGetMethod();
            FieldInfo contextLerpPercent = AccessTools.Field(RenderMeshContext, "lerp_percent");
            if (target == default(MethodInfo))
                Debug.LogWarning($"[Pressurized] Could not find ConduitFlowVisualizer.CalculateMassScale() MethodBase");

            if (RenderMeshContext == null)
                Debug.LogWarning($"[Pressurized] Could not find Type for RenderMeshContext!!");

            if (Context_Outer == null)
                Debug.LogWarning($"[Pressurized] Could not find FieldInfo for outer context!");

            bool foundFirst = false;
            foreach (CodeInstruction original in instructions)
            {
                if (original.opcode == OpCodes.Callvirt && (original.operand as MethodInfo) == target)
                {
                    if (!foundFirst)//moving
                    {
                        //DESCRIPTION: The original purpose of this specific piece of code being modified is to determine the size of the ball visible when looking at the respective conduit overlay.
                        //For example, if using the ventillation overlay, the sizes could range between 1g of gass to 1000g of gas, and the sizes of the balls visbible will scale dependant on that range
                        //However, the pressurized pipes have drastically higher content sizes, but by default will still only scale on a range of 1g to 1000g (for gas conduits).
                        //Even though 1000g of gas is only 1/3 the capacity of a pressurized pipe, the ball will have the max size possible, making it appear as though it is 3000g of gas
                        //To override this, change the apparent mass in a conduit that is pressurized.
                        //For example, if 2100g of gas is present in a pressurized gas pipe, modify the mass to appear as if it were 700g instead when determining the size of the ball. (1-3000g scale to 1-1000g scale).
                        //Additionally, take into consideration both the scaling of the pipe the mass is currently in as well as the mass of the destination.
                        //If moving to a lower or higher capacity pipe, adjust the scale over time as the ball visually moves from one to the next (using the context.lerp_percent field)

                        //change: context.outer.CalculateMassScale(x)
                        //TO: 
                        //LerpMasses(context.outer.CalculateMassScale(ModifyApparentMass(x, cell, context.outer.flowManager.conduitType)), 
                        //  context.outer.CalculateMassScale(ModifyApparentMass(lastFlowInfo.contents.mass, cellFromDirection, context.outer.flowManager.conduitType)))
                        foundFirst = true;
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 5); //cell
                        yield return new CodeInstruction(OpCodes.Ldarg_1); //context
                        yield return new CodeInstruction(OpCodes.Ldfld, Context_Outer); //context.outer
                        yield return new CodeInstruction(OpCodes.Ldfld, flowManager); //context.outer.flowManager
                        yield return new CodeInstruction(OpCodes.Ldfld, conduitType); //context.outer.flowManager.conduitType
                        yield return new CodeInstruction(OpCodes.Call, massPatch); //ModifyApparentMass(x, cell, context.outer.flowManager.conduitType)
                        yield return original; //context.outer.CalculateMassScale(ModifyApparentMass(x, cellFromDirection, context.outer.flowManager.conduitType))

                        //context.outer.CalculateMassScale(ModifyApparentMass(lastFlowInfo.contents.mass, cellFromDirection, context.outer.flowManager.conduitType))
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldfld, Context_Outer);
                        yield return new CodeInstruction(OpCodes.Ldloca_S, 3); //lastFlowInfo
                        yield return new CodeInstruction(OpCodes.Ldflda, conduitContents); //lastFlowInfo.contents
                        yield return new CodeInstruction(OpCodes.Call, conduitContentsMass); //lastFlowInfo.contents.mass
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 6); //cellFromDirection
                        yield return new CodeInstruction(OpCodes.Ldarg_1); //context.outer.flowManager.conduitType
                        yield return new CodeInstruction(OpCodes.Ldfld, Context_Outer);
                        yield return new CodeInstruction(OpCodes.Ldfld, flowManager);
                        yield return new CodeInstruction(OpCodes.Ldfld, conduitType);
                        yield return new CodeInstruction(OpCodes.Call, massPatch); //ModifyApparentMass(lastFlowInfo.contents.mass, cellFromDirection, context.outer.flowManager.conduitType)
                        yield return original; //context.outer.CalculateMassScale(ModifyApparentMass(lastFlowInfo.contents.mass, cellFromDirection, context.outer.flowManager.conduitType))
                        yield return new CodeInstruction(OpCodes.Ldarg_1); //context
                        yield return new CodeInstruction(OpCodes.Ldfld, contextLerpPercent); //context.lerp_percent
                        yield return new CodeInstruction(OpCodes.Call, lerpPatch); //LerpMasses(context.outer.CalculateMassScale(ModifyApparentMass(x, cell, context.outer.flowManager.conduitType)), 
                        //  context.outer.CalculateMassScale(ModifyApparentMass(lastFlowInfo.contents.mass, cellFromDirection, context.outer.flowManager.conduitType)))
                    }
                    else//static
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 15); //cell2
                        yield return new CodeInstruction(OpCodes.Ldarg_1); //context.outer.flowManager.conduitType
                        yield return new CodeInstruction(OpCodes.Ldfld, Context_Outer);
                        yield return new CodeInstruction(OpCodes.Ldfld, flowManager);
                        yield return new CodeInstruction(OpCodes.Ldfld, conduitType);
                        yield return new CodeInstruction(OpCodes.Call, massPatch); //ModifyApparentMass(x, cell2, context.outer.flowManager.conduitType)
                        yield return original; //context.outer.CalculateMassScale(ModifyApparentMass(x, cell2, context.outer.flowManager.conduitType)
                    }
                }
                else
                {
                    yield return original;
                }
            }
        }

        //If the mass originates from a pressurized conduit, change the mass to fit within the default range.
        private static float ModifyApparentMass(float originalMass, int cell, ConduitType type)
        {
            try
            {
                Pressurized pressure = Integration.GetPressurizedAt(cell, type);
                if (!Pressurized.IsDefault(pressure))
                    return originalMass / pressure.Info.IncreaseMultiplier;

                return originalMass;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Pressurized] Error caught in ModifyApparentMass ->\n{e.Message}");
            }
            return originalMass;
        }

        //Interlopate the scaling if the original scale and the receiving scale differ. This is specifically for the visual of masses moving between conduits in the Gas/Liquid overlays.
        //lerpPecent is already used by the game to interlopate the color between standard/insulated/radiant conduits
        private static float LerpMasses(float originScale, float receiverScale, float lerpPercent)
        {
            return Mathf.Lerp(originScale, receiverScale, lerpPercent);
        }
        private class BiggerCapacityState
        {
            public float maxFlow;
            public float CurrentFlow;
        }
        private static void BiggerCapacityPrefix(ValveBase __instance, out BiggerCapacityState __state)
        {
            __state = new BiggerCapacityState()
            {
                maxFlow = __instance.maxFlow,
                CurrentFlow = __instance.CurrentFlow
            };
        }
    }
}
