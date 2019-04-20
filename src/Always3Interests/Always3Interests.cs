using Database;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TUNING;
using UnityEngine;

namespace Always3Interests
{
    [HarmonyPatch(typeof(AirFilterConfig), "CreateBuildingDef")]
    public class Class1
    {
        public static void Postfix(AirFilterConfig __instance, ref BuildingDef __result)
        {
            __result.PermittedRotations = PermittedRotations.FlipV;
        }
    }
    [HarmonyPatch(typeof(SteamTurbineConfig2), "CreateBuildingDef")]
    public class Class2
    {
        public static void Postfix(SteamTurbineConfig2 __instance, ref BuildingDef __result)
        {
            __result.PermittedRotations = PermittedRotations.R360;
        }
    }
    /*[HarmonyPatch(typeof(MinionStartingStats), "GenerateTraits")]
    public class Class3
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_0)//98
                {
                    codes[i].opcode = OpCodes.Ldc_I4;
                    codes[i].operand = -20;

                }
            }

            return codes.AsEnumerable();
        }
    }*/
    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class Class3
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_1 || codes[i].opcode == OpCodes.Ldc_I4_4)
                {
                    codes[i].opcode = OpCodes.Ldc_I4;
                    codes[i].operand = 3;
                }
                if (i > 3) break;

            }
            for (int i = 0; i < codes.Count; i++)
            {
                Debug.Log("here " + codes[i].ToString());
            }

            return codes.AsEnumerable();
        }
    }


}
