using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace High_Pressure_Applications
{
    internal static partial class HarmonyPatches
    {
        public class ConduitFlowPatches
        {
            //Modify MaxMass if needed for pressurized pipes when adding elements to a pipe
            [HarmonyPatch(typeof(ConduitFlow), "AddElement")]
            internal static class Patch_ConduitFlow_AddElement
            {

                internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    CodeInstruction getCellInstruction = new CodeInstruction(OpCodes.Ldarg_1); //int cell_idx : The first argument of the method being called (Ldarg_0 is the instance (this) reference)
                    foreach (CodeInstruction code in instructions)
                    {
                        foreach (CodeInstruction result in Integration.AddIntegrationIfNeeded(code, getCellInstruction))
                        {
                            yield return result;
                        }
                    }

                }
            }

            //Modify MaxMass if needed for pressurized pipes when update conduits. Also include overpressure integration
            [HarmonyPatch(typeof(ConduitFlow), "UpdateConduit")]
            internal static class Patch_ConduitFlow_UpdateConduit
            {
                internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    //variable: int cell2;
                    //This variable is used for the patch to determine the cell of the conduit being updated. The cell is then used in determining what its MaxMass (max capacity) should be
                    CodeInstruction getCellInstruction = new CodeInstruction(OpCodes.Ldloc_S, 13);
                    foreach (CodeInstruction code in instructions)
                    {
                        foreach (CodeInstruction result in Integration.AddIntegrationIfNeeded(code, getCellInstruction, true))
                        {
                            yield return result;
                        }
                    }

                }
            }

            //Modify MaxMass if needed for pressurized pipes when determining if the conduit is full.
            [HarmonyPatch(typeof(ConduitFlow), "IsConduitFull")]
            internal static class Patch_ConduitFlow_IsConduitFull
            {
                internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    CodeInstruction getCellInstruction = new CodeInstruction(OpCodes.Ldarg_1); //int cell_idx : The first argument of the method being called (Ldarg_0 is the instance (this) reference)
                    foreach (CodeInstruction code in instructions)
                    {
                        foreach (CodeInstruction result in Integration.AddIntegrationIfNeeded(code, getCellInstruction))
                        {
                            yield return result;
                        }
                    }
                }
            }

            //When Deserializing the contents inside of Conduits, the method will normally prevent the deserialized data from being higher than the built-in ConduitFlow MaxMass.
            //Instead, replace the max mass with infinity so the serialized mass will always be used.
            //Must be done this way because OnDeserialized is called before the Conduits are spawned, so no information is available as to what the max mass is supposed to be
            [HarmonyPatch(typeof(ConduitFlow), "OnDeserialized")]
            internal static class Patch_ConduitFlow_OnDeserialized
            {
                internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    MethodInfo patch = AccessTools.Method(typeof(Patch_ConduitFlow_OnDeserialized), "ReplaceMaxMass");
                    foreach (CodeInstruction original in instructions)
                    {
                        if (original.opcode == OpCodes.Ldfld && (original.operand as FieldInfo) == maxMass)
                        {
                            yield return original;
                            yield return new CodeInstruction(OpCodes.Call, patch);
                        }
                        else
                            yield return original;
                    }
                }

                internal static float ReplaceMaxMass(float original)
                {
                    return float.PositiveInfinity;
                }
            }
        }
    }
}
