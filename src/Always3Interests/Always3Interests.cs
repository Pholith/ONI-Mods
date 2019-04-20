using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ONI_Common.Json;

namespace Always3Interests
{
    

    public class Config
    {
        public static BaseStateManager<Config> StateManager = new BaseStateManager<Config>("Always3Interests");
        public int numberOfInterests { get; set; } = 3;
    }
    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]




    public class Always3Interests
    {
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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


}
