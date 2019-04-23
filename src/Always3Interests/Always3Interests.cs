using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ONI_Common.Json;
using UnityEngine;

namespace Always3Interests
{
    
    public class Config
    {
        public static BaseStateManager<Config> StateManager = new BaseStateManager<Config>("Always3Interests");
        public int numberOfInterests { get; set; } = 3;
        public bool moreGoodTraits { get; set; } = false;
        public bool moreBadTraits { get; set; } = false;
        public int bonusStats { get; set; } = 0;
    }


    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
    public class GenerateAptitudes
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
        int numberOfInterests = Config.StateManager.State.numberOfInterests;
        
        var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_1 || codes[i].opcode == OpCodes.Ldc_I4_4)
                {
                    codes[i].opcode = OpCodes.Ldc_I4;
                    codes[i].operand = numberOfInterests;
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
    [HarmonyPatch(typeof(MinionStartingStats), "GenerateTraits")]
    public class GenerateTraits
    {
        //[HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            codes[12].opcode = OpCodes.Ldc_I4;
            Debug.Log(Config.StateManager.State.bonusStats);
            codes[12].operand = Config.StateManager.State.bonusStats;

            if (Config.StateManager.State.moreBadTraits)
            {
                codes[63].opcode = OpCodes.Ldc_I4;
                codes[63].operand = -3;
            }
            if (Config.StateManager.State.moreGoodTraits)
            {
                codes[86].opcode = OpCodes.Ldc_I4;
                codes[86].operand = -3;
            }
            for (int i = 0; i < codes.Count; i++)
            {
                Debug.Log("here " + i + " " + codes[i].ToString());
            }
            return codes.AsEnumerable();
        }
    }
}
