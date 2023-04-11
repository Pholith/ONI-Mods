using HarmonyLib;
using Pholib;

namespace CustomizeYourPaints.Art
{

    public class ArtablePatches
    {
        [HarmonyPatch(typeof(Artable), "OnSpawn")]
        public class Artable_OnSpawn_Patch
        {
            public static void Prefix(Artable __instance, ref string ___currentStage)
            {
                ArtHelper.RestoreStage(__instance, ref ___currentStage);
            }
        }

        [HarmonyPatch(typeof(Artable), "SetStage")]
        public class Artable_SetStage_Patch
        {
            public static void Prefix(Artable __instance, ref string stage_id)
            {
                ArtHelper.UpdateOverride(__instance, stage_id);
            }
        }

        [HarmonyPatch(typeof(Artable), "OnDeserialized")]
        public class Artable_OnDeserialized_Patch
        {
            // prevent invalid stages
            public static void Postfix(string ___defaultArtworkId, ref string ___currentStage)
            {
                if (Db.GetArtableStages().TryGet(___currentStage) == null)
                {
                    ___currentStage = ___defaultArtworkId;
                }
            }
        }
    }

}