using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TUNING;
using UnityEngine;
using Pholib;

namespace Always3Interests
{

    [HarmonyPatch(typeof(Db), "Initialize")]
    class TuningConfigPatch
    {
        public static JsonReader config;

        public static void Prefix()
        {

            config = new JsonReader();

            var custom1 = config.GetProperty<int>("pointsWhen1Interest", 7);
            var custom2 = config.GetProperty<int>("pointsWhen2Interest", 3);
            var custom3 = config.GetProperty<int>("pointsWhen3Interest", 1);

            var customAttributes = new int[] { custom1, custom2, custom3 };
            Traverse.Create<DUPLICANTSTATS>().Field("APTITUDE_ATTRIBUTE_BONUSES").SetValue(customAttributes);
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class GenerateAptitudesPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            JsonReader config = new JsonReader();

            var numberOfInterests = config.GetProperty<int>("numberOfInterests", 3);

            for (int i = 0; i < 5; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_1)
                {
                    codes[i].opcode = OpCodes.Ldc_I4;
                    codes[i].operand = numberOfInterests;
                }
                if (codes[i].opcode == OpCodes.Ldc_I4_4)
                {
                    codes[i].opcode = OpCodes.Ldc_I4;
                    codes[i].operand = numberOfInterests;
                }
                //Debug.Log("test= " + codes[i].ToString());
            }
            return codes.AsEnumerable();
        }
    }
}
