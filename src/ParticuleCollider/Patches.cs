using HarmonyLib;
using Pholib;

namespace ParticleCollider
{
    public static class Patches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class ImplementationPatch
        {
            private static void Prefix()
            {
                Utilities.AddBuilding("Refining", ParticleColliderConfig.ID, "test", "test", "test");// PHO_STRINGS.HELIUMEXTRACTOR.NAME, PHO_STRINGS.HELIUMEXTRACTOR.DESC, PHO_STRINGS.HELIUMEXTRACTOR.EFFECT);
            }

        }

        [HarmonyPatch(typeof(UraniumCentrifugeConfig))]
        [HarmonyPatch(nameof(UraniumCentrifugeConfig.ConfigureBuildingTemplate))]
        public static class NuclearWasteRecyclePatch
        {
            private static void Postfix()
            {
                Utilities.AddComplexRecipe(
                    new ComplexRecipe.RecipeElement[] { new ComplexRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), 200) },
                    new ComplexRecipe.RecipeElement[] { new ComplexRecipe.RecipeElement(SimHashes.Niobium.CreateTag(), 1) },
                    // new ComplexRecipe.RecipeElement(SimHashes.Silver / Palladium .CreateTag(), 1),
                    UraniumCentrifugeConfig.ID,
                    50f, "", ComplexRecipe.RecipeNameDisplay.Result, 200);

            }

        }

    }
}
