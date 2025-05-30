using HarmonyLib;
using Klei.AI;
using Pholib;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

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
                Utilities.AddBuilding("Refining", ParticleColliderConfig.ID, "ParticleCollider", "test", "test");// PHO_STRINGS.HELIUMEXTRACTOR.NAME, PHO_STRINGS.HELIUMEXTRACTOR.DESC, PHO_STRINGS.HELIUMEXTRACTOR.EFFECT);
            }

        }

        [HarmonyPatch(typeof(UraniumCentrifugeConfig))]
        [HarmonyPatch(nameof(UraniumCentrifugeConfig.ConfigureBuildingTemplate))]
        public static class NuclearWasteRecyclePatch
        {
            private static void Postfix()
            {
                float powerToGasAmount = 10f;
                var powerToGas = Utilities.AddComplexRecipe(
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.CarbonDioxide.CreateTag(), powerToGasAmount, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                        new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), powerToGasAmount, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true),
                    },
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.Methane.CreateTag(), 0.11199999f * powerToGasAmount, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                        new ComplexRecipe.RecipeElement(SimHashes.Oxygen.CreateTag(), 0.888f * powerToGasAmount, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false),
                        new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), powerToGasAmount, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true),
                    }, ChemicalRefineryConfig.ID, 60, STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION, ComplexRecipe.RecipeNameDisplay.Custom, 200);
                powerToGas.customName = "Power to Gas";

                Utilities.AddComplexRecipe(
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 70f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                        new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 30f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                    },
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.Brine.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    }, ChemicalRefineryConfig.ID, 40, STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION, ComplexRecipe.RecipeNameDisplay.Result, 150);


                Element silver = ElementLoader.FindElementByName("Silver");
                Element palladium = ElementLoader.FindElementByName("Palladium");
                Element helium = ElementLoader.FindElementByHash(SimHashes.Helium);
                Element uranium = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium);

                float recipeAmount = 1000; // 1000 kg
                var results = new ComplexRecipe.RecipeElement[] {
                    new ComplexRecipe.RecipeElement(uranium.id.CreateTag(), recipeAmount / 100 / 2) // 1kg of enriched uranium gives 100 kg of waste
                    };

                if (silver != null)
                {
                    results = results.Append(new ComplexRecipe.RecipeElement(silver.id.CreateTag(), 1 / 100 * recipeAmount));
                }
                if (palladium != null)
                {
                    results = results.Append(new ComplexRecipe.RecipeElement(palladium.id.CreateTag(), 8 / 100 * recipeAmount));
                }
                if (helium != null)
                {
                    results = results.Append(new ComplexRecipe.RecipeElement(helium.id.CreateTag(), 0.4f / 100 * recipeAmount));
                }

                if (results.Length > 0)
                {
                    var recipe = Utilities.AddComplexRecipe(
                        new ComplexRecipe.RecipeElement[] { new ComplexRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), recipeAmount) },
                        results,
                        UraniumCentrifugeConfig.ID,
                        200f, "", ComplexRecipe.RecipeNameDisplay.Custom, 200);
                    recipe.customName = "Nuclear Waste Recycling";
                    recipe.customSpritePrefabID = SimHashes.NuclearWaste.CreateTag().ToString();
                    //recipe.customSpritePrefabID = ElementLoader.FindElementByHash(SimHashes.NuclearWaste).
                }

            }

        }

        ////// ColliderRecipe patchs
        [HarmonyPatch(typeof(SelectedRecipeQueueScreen))]
        [HarmonyPatch(nameof(SelectedRecipeQueueScreen.SetRecipeCategory))]
        public static class SelectedRecipeQueueScreenPatch
        {
            private static void Postfix(SelectedRecipeQueueScreen __instance)
            {
                ComplexRecipe selectedRecipe = Traverse.Create(__instance)?.Property("firstSelectedRecipe")?.GetValue<ComplexRecipe>();
                if (selectedRecipe != null && selectedRecipe is ColliderRecipe colliderRecipe)
                {
                    string text = selectedRecipe.time.ToString() + " " + UI.UNITSUFFIXES.SECONDS.ToString().ToLower() + "                                     " + Utilities.FormatColored(colliderRecipe.energyRequired + " " + UI.UNITSUFFIXES.ELECTRICAL.WATT.ToString(), "c08210");
                    __instance.recipeDuration.SetText(text);
                }
            }
        }
        [HarmonyPatch(typeof(ComplexRecipeManager))]
        [HarmonyPatch(nameof(ComplexRecipeManager.Add))]
        public static class ComplexRecipeManagerPatch
        {
            private static bool Prefix(ComplexRecipeManager __instance, ComplexRecipe recipe, bool real)
            {
                if (recipe is ColliderRecipe)
                {
                    __instance.recipes.Add(recipe);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(ComplexFabricator))]
        [HarmonyPatch("StartWorkingOrder")]
        public static class ComplexFabricatorPatch
        {
            public static void Postfix(ComplexFabricator __instance)
            {
                if (__instance is ParticleCollider)
                {
                    BuildingDef buildingDef = __instance.gameObject.GetComponent<Building>().Def;
                    buildingDef.EnergyConsumptionWhenActive = (__instance.CurrentWorkingOrder as ColliderRecipe).energyRequired;

                    __instance.gameObject.GetComponent<EnergyConsumer>().BaseWattageRating = (__instance.CurrentWorkingOrder as ColliderRecipe).energyRequired;
                }
            }
        }

        //// Add disease
        [HarmonyPatch(typeof(ComplexFabricator))]
        [HarmonyPatch("SpawnOrderProduct")]
        public static class ComplexFabricator_SpawnOrderProduct_Patch
        {
            public static void Postfix(ComplexFabricator __instance, List<GameObject> __result, ComplexRecipe recipe)
            {
                if (recipe is ColliderRecipe colliderRecipe)
                {
                    foreach (GameObject recipeResult in __result)
                    {
                        PrimaryElement primaryElement = recipeResult.GetComponent<PrimaryElement>();
                        primaryElement.SetUseSimDiseaseInfo(false);
                        primaryElement.AddDisease(Db.Get().Diseases.GetIndex(RadiationPoisoning.ID), colliderRecipe.radiationCreated, "ParticleCollider.Emit");
                    }
                }
            }
        }
    }
}
