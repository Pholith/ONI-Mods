using HarmonyLib;
using Pholib;

namespace CustomizeYourPaints.Art
{
    public class ArtHelper
    {
        public static void RestoreStage(Artable instance, ref string currentStage)
        {
            ArtOverride artOverride;
            if (instance.TryGetComponent<ArtOverride>(out artOverride) && !artOverride.overrideStage.IsNullOrWhiteSpace()) // test if mod is active
            {
                if (CustomizeYourPaints.myOverrides.Contains(artOverride.overrideStage)) 
                    currentStage = artOverride.overrideStage;
                else if (CustomizeYourPaints.myOverrides.Count > 0)
                {
                    string searchName = artOverride.overrideStage.Split('_')[1];
                    Logs.Log("search name = " + searchName);
                    currentStage = CustomizeYourPaints.myOverrides.Find((txt) => txt == searchName);

                    if (currentStage == null )
                    {
                        currentStage = CustomizeYourPaints.myOverrides.GetRandom();
                    }
                }
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

        public ArtHelper()
        {
        }
    }
}
