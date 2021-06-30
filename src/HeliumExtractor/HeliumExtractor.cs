using HarmonyLib;
using static OilRefinery;

namespace HeliumExtractor
{
    class HeliumExtractor : OilRefinery
    {
    }

    [HarmonyPatch(typeof(WorkableTarget))]
    [HarmonyPatch("OnPrefabInit")]
    public class AnimationPatch
    {
        public static void Postfix(WorkableTarget __instance)
        {
            if (__instance.name == "HeliumExtractorComplete")
            {
                // Fix the OilRefinery animation 
                __instance.overrideAnims = new KAnimFile[]
                {
                    Assets.GetAnim("anim_interacts_metalrefinery_kanim")
                };
            }
        }
    }
}
