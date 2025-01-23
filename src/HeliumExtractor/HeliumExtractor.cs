using HarmonyLib;
using Pholib;
using static OilRefinery;

namespace HeliumExtractor
{

    public class HeliumExtractor : OilRefinery
    {
    }

    [HarmonyPatch(typeof(WorkableTarget))]
    [HarmonyPatch("OnPrefabInit")]
    public class HeliumExtractor_Animation_Patch
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
