using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;
using High_Pressure_Applications.Components;

namespace High_Pressure_Applications
{
    internal class VisualPatches
    {
        //Prevent the game from marking our pipes as radiant or insulated. Otherwise, overlay tints will not properly appear.
        [HarmonyPatch(typeof(ConduitFlowVisualizer), "AddThermalConductivity")]
        internal static class Patch_ConduitFlowVisualizer_AddThermalConductivity
        {
            private static readonly FieldInfo conduitType = AccessTools.Field(typeof(ConduitFlow), "conduitType");

            internal static void Prefix(ConduitFlowVisualizer __instance, int cell, ref float conductivity, ConduitFlow ___flowManager)
            {
                Pressurized pressure = Integration.GetPressurizedAt(cell, (ConduitType)conduitType.GetValue(___flowManager));
                if (!Pressurized.IsDefault(pressure))
                    conductivity = 1f;
            }
        }
        //Prevent the game from attempting to remove our pipes from their list of radiant/insulated pipes, since our pipes will not be in those lists in the first place
        [HarmonyPatch(typeof(ConduitFlowVisualizer), "RemoveThermalConductivity")]
        internal static class Patch_ConduitFlowVisualizer_RemoveThermalConductivity
        {
            private static readonly FieldInfo conduitType = AccessTools.Field(typeof(ConduitFlow), "conduitType");

            internal static void Prefix(ConduitFlowVisualizer __instance, int cell, ref float conductivity, ConduitFlow ___flowManager)
            {
                Pressurized pressure = Integration.GetPressurizedAt(cell, (ConduitType)conduitType.GetValue(___flowManager));
                if (!Pressurized.IsDefault(pressure))
                    conductivity = 1f;
            }
        }


        [HarmonyPatch(typeof(ConduitFlowVisualizer), "GetCellTintColour")]
        internal static class Patch_ConduitFlowVisualizer_GetCellTintColour
        {
            private static readonly FieldInfo conduitType = AccessTools.Field(typeof(ConduitFlow), "conduitType");

            internal static void Postfix(ConduitFlowVisualizer __instance, int cell, ConduitFlow ___flowManager, bool ___showContents, ref Color32 __result)
            {
                Pressurized pressure = Integration.GetPressurizedAt(cell, (ConduitType)conduitType.GetValue(___flowManager));
                if (!Pressurized.IsDefault(pressure))
                    __result = ___showContents ? pressure.Info.FlowOverlayTint : pressure.Info.FlowTint;
            }
        }

        //To change the color of how our pressurized pipes are displayed in their respective overlay
        [HarmonyPatch(typeof(OverlayModes.ConduitMode), "Update")]
        internal static class Patch_OvererlayModesConduitMode_Update
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
            {
                MethodBase patch = AccessTools.Method(typeof(Patch_OvererlayModesConduitMode_Update), nameof(PatchThermalColor));
                //SaveLoadRoot layerTarget;
                int layerTargetIdx = 12;
                //Color32 color;
                int tintColourIdx = 14;
                bool foundVar = false;
                LocalVariableInfo layerTargetInfo = original.GetMethodBody().LocalVariables.FirstOrDefault(x => x.LocalIndex == layerTargetIdx);
                foundVar = layerTargetInfo != default(LocalVariableInfo);
                if (!foundVar)
                    Debug.LogError($"[Pressurized] OverlayModes.ConduitMode.Update() Transpiler -> Local variable signatures did not match expected signatures");

                foreach (CodeInstruction code in instructions)
                {
                    if (foundVar && code.opcode == OpCodes.Stloc_S && (code.operand as LocalVariableInfo)?.LocalIndex == tintColourIdx)
                    {
                        //PatchThermalColor(color, layerTarget)
                        yield return new CodeInstruction(OpCodes.Ldloc_S, layerTargetIdx);
                        yield return new CodeInstruction(OpCodes.Call, patch);
                    }
                    yield return code;
                }
            }

            //Change the overlay tint for the pipe if it is a pressurized pipe.
            private static Color32 PatchThermalColor(Color32 original, SaveLoadRoot layerTarget)
            {
                Pressurized pressurized = layerTarget.GetComponent<Pressurized>();
                if (pressurized != null && pressurized.Info != null && !pressurized.Info.IsDefault)
                    return pressurized.Info.OverlayTint;
                else
                    return original;
            }
        }

    }
}
