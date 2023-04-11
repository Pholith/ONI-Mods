using HarmonyLib;

namespace CustomizeYourPaints.Art
{
    public class ArtHelper
    {
        public static void RestoreStage(Artable instance, ref string currentStage)
        {
            ArtOverride artOverride;
            if (instance.TryGetComponent<ArtOverride>(out artOverride) && !artOverride.overrideStage.IsNullOrWhiteSpace())
            {
                currentStage = artOverride.overrideStage;
            }
        }

        public static void UpdateOverride(Artable instance, string stage_id)
        {
            ArtOverride artOverride;
            if (instance.TryGetComponent<ArtOverride>(out artOverride))
            {
                artOverride.UpdateOverride(stage_id);
            }
        }

        [HarmonyPatch(typeof(Artable), "OnSpawn")]
        public class CustomPaints_Artable_OnSpawn_Patch
        {
            public static void Prefix(Artable __instance, ref string ___currentStage, string ___defaultArtworkId)
            {

            }
        }

        public ArtHelper()
        {
        }
    }
}
