using HarmonyLib;
using Klei.AI;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using Pholib;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ParticleCollider
{

    public class HighTechMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            //new POptions().RegisterOptions(this, typeof(NotepadOptions));
            //GameOnLoadPatch.ReadSettings(); // Read settings early for the notepad description setting.

            new PLocalization().Register();
            Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));
        }
    }

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

            public static Element silver = ElementLoader.FindElementByName("SolidSilver") ?? ElementLoader.FindElementByName("Silver");
            public static Element palladium = ElementLoader.FindElementByName("Palladium");
            public static Element helium = ElementLoader.FindElementByHash(SimHashes.Helium);
            public static Element uranium = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium);
            public static Element depletedUranium = ElementLoader.FindElementByHash(SimHashes.DepletedUranium);

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


                float recipeAmount = 1000; // 1000 kg
                var results = new ComplexRecipe.RecipeElement[] {
                    new ComplexRecipe.RecipeElement(depletedUranium.id.CreateTag(), 50f / 100f * recipeAmount), // 1kg of enriched uranium gives 100 kg of waste
                    new ComplexRecipe.RecipeElement(uranium.id.CreateTag(), recipeAmount / 100f / 2f) // 1kg of enriched uranium gives 100 kg of waste
                    };

                if (silver != null)
                {
                    // Compatibility with Chemical Processing: Industrial Overhaul Edition
                    results = results.Append(new ComplexRecipe.RecipeElement(silver.id.CreateTag(), 1f / 100f * recipeAmount));
                }
                if (palladium != null)
                {
                    results = results.Append(new ComplexRecipe.RecipeElement(palladium.id.CreateTag(), 8f / 100f * recipeAmount));
                }
                if (helium != null)
                {
                    results = results.Append(new ComplexRecipe.RecipeElement(helium.id.CreateTag(), 0.2f / 100f * recipeAmount));
                }

                if (results.Length > 0)
                {
                    var recipe = Utilities.AddComplexRecipe(
                        new ComplexRecipe.RecipeElement[] { new ComplexRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), recipeAmount) },
                        results,
                        UraniumCentrifugeConfig.ID,
                        150f, "", ComplexRecipe.RecipeNameDisplay.Custom, 200);
                    recipe.customName = "Nuclear Waste Recycling";
                    recipe.customSpritePrefabID = SimHashes.NuclearWaste.CreateTag().ToString();
                    //recipe.customSpritePrefabID = ElementLoader.FindElementByHash(SimHashes.NuclearWaste).
                }

            }

        }
        // Make Centrifuger drop output
        [HarmonyPatch(typeof(UraniumCentrifuge))]
        [HarmonyPatch("DropEnrichedProducts")]
        public static class UraniumCentrifugeConfig_DropPatch
        {
            private static void Postfix(UraniumCentrifuge __instance)
            {
                Storage[] components = __instance.GetComponents<Storage>();
                foreach (Storage storage in components)
                {
                    if (NuclearWasteRecyclePatch.silver != null) storage.Drop(NuclearWasteRecyclePatch.silver.tag);
                    if (NuclearWasteRecyclePatch.palladium != null) storage.Drop(NuclearWasteRecyclePatch.palladium.tag);
                    if (NuclearWasteRecyclePatch.helium != null) storage.Drop(NuclearWasteRecyclePatch.helium.tag);
                    if (NuclearWasteRecyclePatch.depletedUranium != null) storage.Drop(NuclearWasteRecyclePatch.depletedUranium.tag);
                }
            }
        }
        // Make Centrifuger sealed for nuclear waste
        [HarmonyPatch(typeof(UraniumCentrifugeConfig))]
        [HarmonyPatch(nameof(UraniumCentrifugeConfig.ConfigureBuildingTemplate))]
        public static class UraniumCentrifugeConfig_SealPatch
        {
            private static void Prefix(GameObject go, Tag prefab_tag, ref List<Storage.StoredItemModifier> ___storedItemModifiers)
            {
                Logs.Log(___storedItemModifiers);
                ___storedItemModifiers.Add(Storage.StoredItemModifier.Seal);
            }
        }

        // Patch helium wrong values
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch("CopyEntryToElement")]
        public static class ElementLoader_PatchHeliumValues
        {
            private static void Postfix(ElementLoader.ElementEntry entry, Element elem)
            {
                if (elem.id == SimHashes.Helium)
                {
                    elem.thermalConductivity = 0.15f;
                    elem.specificHeatCapacity = 5.193f;
                }
                if (elem.id == SimHashes.Helium || elem.id == SimHashes.LiquidHelium)
                {
                    elem.disabled = false;
                    elem.oreTags = elem.oreTags.Except(new Tag[] { GameTags.HideFromCodex, GameTags.HideFromSpawnTool }).Cast<Tag>().ToArray(); // Remove Propane Hide tags
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
