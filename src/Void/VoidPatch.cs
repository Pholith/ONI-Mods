using Harmony;
using System.Reflection;
using UnityEngine;

namespace VoidPatch
{
    [HarmonyPatch(typeof(Db), "Initialize")]
    class VoidPatch
    {

        public static LocString NAME = "Void";
        public static LocString DESCRIPTION = "Description of void. </smallcaps>\n\n";

        public static void Prefix()
        {
            // Add strings used in Fuleria.yaml
            Strings.Add($"STRINGS.WORLDS.VOID.NAME", NAME);
            Strings.Add($"STRINGS.WORLDS.VOID.DESCRIPTION", DESCRIPTION);

            // Generate a translation .pot 
            ModUtil.RegisterForTranslation(typeof(VoidPatch));

        }
    }
}
