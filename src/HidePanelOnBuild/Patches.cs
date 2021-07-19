using HarmonyLib;

namespace HidePanelOnBuild
{
    public class Patches
    {
        [HarmonyPatch(typeof(BuildToolHoverTextCard))]
        [HarmonyPatch("UpdateHoverElements")]
        public class HoverText_ConfigureTitlePatch
        {
            public static bool Prefix()
            {
                return false;
            }
        }

    }
}
