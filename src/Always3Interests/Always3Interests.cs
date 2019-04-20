using Database;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TUNING;
using UnityEngine;

namespace Always3Interests
{
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
            /*for (int i = 0; i < codes.Count; i++)
            {
                Debug.Log("here " + codes[i].ToString());
            }*/

            return codes.AsEnumerable();
        }
    }


}
