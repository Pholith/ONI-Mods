using HarmonyLib;
using Klei.AI;
using KMod;
using PeterHan.PLib.Database;
using Pholib;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HighTechIndustry
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
                Utilities.AddBuilding("Refining", NeutronicTransmutationChamberConfig.ID, PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.NAME, PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.DESC, PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.EFFECT);
            }
        }

        [HarmonyPatch(typeof(UraniumCentrifugeConfig))]
        [HarmonyPatch(nameof(UraniumCentrifugeConfig.ConfigureBuildingTemplate))]
        public static class HighTechIndustry_NuclearWasteRecyclePatch
        {
            public static Element uranium = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium);
            public static Element depletedUranium = ElementLoader.FindElementByHash(SimHashes.DepletedUranium);

            // Compatibility with Ronivan's Legacy Industrial Revolution
            public static Element silver = ElementLoader.FindElementByName("SolidSilver") ?? ElementLoader.FindElementByName("Silver");
            public static Element palladium = ElementLoader.FindElementByName("Palladium");
            public static Element nitrogen = ElementLoader.FindElementByName("NitrogenGas");

            // HighTechIndustry_NuclearWasteRecyclePatch.helium disabled in game but reabled by HighTechIndustry_NuclearWasteRecyclePatch.helium Extractor
            public static Element helium = ElementLoader.FindElementByHash(SimHashes.Helium);


            private static void Postfix()
            {

                float powerToGasAmount = 100f;
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
                    }, ChemicalRefineryConfig.ID, 60, PHO_STRINGS.RECIPE.POWER_TO_METHANE_DESC, ComplexRecipe.RecipeNameDisplay.Custom, 200, Db.Get().TechItems.superLiquids.parentTechId);
                powerToGas.customName = PHO_STRINGS.RECIPE.POWER_TO_METHANE_NAME;
                powerToGas.customSpritePrefabID = ElementLoader.FindElementByHash(SimHashes.Methane).id.ToString();
                //powerToGas.customSpritePrefabID = SimHashes.Methane.CreateTag().ToString();


                // Adds a recipe for Brine
                Utilities.AddComplexRecipe(
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 70f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                        new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 30f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                    },
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.Brine.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    }, ChemicalRefineryConfig.ID, 40, STRINGS.ELEMENTS.SALTWATER.DESC, ComplexRecipe.RecipeNameDisplay.Result, 150);

                // Adds a recipe for polluted brine
                Utilities.AddComplexRecipe(
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.DirtyWater.CreateTag(), 70f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                        new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 30f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                    },
                    new ComplexRecipe.RecipeElement[]
                    {
                        new ComplexRecipe.RecipeElement(SimHashes.MurkyBrine.CreateTag(), 100, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    }, ChemicalRefineryConfig.ID, 40, STRINGS.ELEMENTS.MURKYBRINE.DESC, ComplexRecipe.RecipeNameDisplay.Result, 150);


                float recipeAmount = 1000; // 1000 kg
                var results = new ComplexRecipe.RecipeElement[] {
                    new ComplexRecipe.RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.depletedUranium.id.CreateTag(), 50f / 100f * recipeAmount), // 1kg of enriched uranium gives 100 kg of waste
                    new ComplexRecipe.RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.uranium.id.CreateTag(), recipeAmount / 100f / 2f) // 1kg of enriched uranium gives 100 kg of waste
                    };


                // Sub product of fission: https://en.wikipedia.org/wiki/Fission_products_(by_element)
                if (HighTechIndustry_NuclearWasteRecyclePatch.silver.ElementExistsAndIsActive())
                {
                    // Compatibility with Chemical Processing: Industrial Overhaul Edition
                    results = results.Append(new ComplexRecipe.RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.silver.id.CreateTag(), 1f / 100f * recipeAmount));
                }
                if (HighTechIndustry_NuclearWasteRecyclePatch.palladium.ElementExistsAndIsActive())
                {
                    results = results.Append(new ComplexRecipe.RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.palladium.id.CreateTag(), 8f / 100f * recipeAmount));
                }
                if (HighTechIndustry_NuclearWasteRecyclePatch.helium.ElementExistsAndIsActive())
                {
                    results = results.Append(new ComplexRecipe.RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.helium.id.CreateTag(), 0.2f / 100f * recipeAmount));
                }

                if (results.Length > 0)
                {
                    var recipe = Utilities.AddComplexRecipe(
                        new ComplexRecipe.RecipeElement[] { new ComplexRecipe.RecipeElement(SimHashes.NuclearWaste.CreateTag(), recipeAmount) },
                        results,
                        UraniumCentrifugeConfig.ID,
                        150f, PHO_STRINGS.RECIPE.NUCLEAR_WASTE_RECYCLING_DESC, ComplexRecipe.RecipeNameDisplay.Custom, 200);
                    recipe.customName = PHO_STRINGS.RECIPE.NUCLEAR_WASTE_RECYCLING_NAME;
                    recipe.customSpritePrefabID = SimHashes.NuclearWaste.CreateTag().ToString();
                    //recipe.customSpritePrefabID = ElementLoader.FindElementByHash(SimHashes.NuclearWaste).
                }

            }

        }
        // Make Centrifuger drop output
        [HarmonyPatch(typeof(UraniumCentrifuge))]
        [HarmonyPatch("DropEnrichedProducts")]
        public static class HighTechIndustry_UraniumCentrifugeConfig_DropPatch
        {
            public static void Postfix(UraniumCentrifuge __instance)
            {
                Storage[] components = __instance.GetComponents<Storage>();
                foreach (Storage storage in components)
                {
                    if (HighTechIndustry_NuclearWasteRecyclePatch.silver.ElementExistsAndIsActive()) storage.Drop(HighTechIndustry_NuclearWasteRecyclePatch.silver.tag);
                    if (HighTechIndustry_NuclearWasteRecyclePatch.palladium.ElementExistsAndIsActive()) storage.Drop(HighTechIndustry_NuclearWasteRecyclePatch.palladium.tag);
                    if (HighTechIndustry_NuclearWasteRecyclePatch.helium.ElementExistsAndIsActive()) storage.Drop(HighTechIndustry_NuclearWasteRecyclePatch.helium.tag);
                    if (HighTechIndustry_NuclearWasteRecyclePatch.depletedUranium.ElementExistsAndIsActive()) storage.Drop(HighTechIndustry_NuclearWasteRecyclePatch.depletedUranium.tag);
                }
            }
        }
        // Make Centrifuger sealed for nuclear waste
        [HarmonyPatch(typeof(UraniumCentrifugeConfig))]
        [HarmonyPatch(nameof(UraniumCentrifugeConfig.ConfigureBuildingTemplate))]
        public static class HighTechIndustry_UraniumCentrifugeConfig_SealPatch
        {
            private static void Prefix(GameObject go, Tag prefab_tag, ref List<Storage.StoredItemModifier> ___storedItemModifiers)
            {
                Logs.LogIfDebugging(___storedItemModifiers);
                ___storedItemModifiers.Add(Storage.StoredItemModifier.Seal);
            }
        }

        // Patch HighTechIndustry_NuclearWasteRecyclePatch.helium wrong values and enable it
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch("CopyEntryToElement")]
        public static class HighTechIndustry_ElementLoader_PatchHelium
        {
            public static void Postfix(ElementData.ElementEntry entry, Element elem)
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
        ////// NeutronicTransmutationRecipe add wattage patchs
        [HarmonyPatch(typeof(SelectedRecipeQueueScreen))]
        [HarmonyPatch(nameof(SelectedRecipeQueueScreen.SetRecipeCategory))]
        public static class HighTechIndustry_SelectedRecipeQueueScreenPatch
        {
            private static void Postfix(SelectedRecipeQueueScreen __instance)
            {
                ComplexRecipe selectedRecipe = Traverse.Create(__instance)?.Property("firstSelectedRecipe")?.GetValue<ComplexRecipe>();
                if (selectedRecipe != null && selectedRecipe is NeutronicTransmutationRecipe colliderRecipe)
                {
                    string text = selectedRecipe.time.ToString() + " " + UI.UNITSUFFIXES.SECONDS.ToString().ToLower() + "                                     " + Utilities.FormatColored(colliderRecipe.energyRequired + " " + UI.UNITSUFFIXES.ELECTRICAL.WATT.ToString(), "c08210");
                    __instance.recipeDuration.SetText(text);
                }
            }
        }

        // Add custom subclass NeutronicTransmutationRecipe to the recipes list, I guess ? I don't remember why I needed this patch
        [HarmonyPatch(typeof(ComplexRecipeManager))]
        [HarmonyPatch(nameof(ComplexRecipeManager.Add))]
        public static class ComplexRecipeManagerPatch
        {
            private static bool Prefix(ComplexRecipeManager __instance, ComplexRecipe recipe, bool real)
            {
                if (recipe is NeutronicTransmutationRecipe)
                {
                    __instance.recipes.Add(recipe);
                    return false;
                }
                return true;
            }
        }

        // Update energy consumption depending on the recipe
        [HarmonyPatch(typeof(ComplexFabricator))]
        [HarmonyPatch("StartWorkingOrder")]
        public static class ComplexFabricatorPatch
        {
            public static void Postfix(ComplexFabricator __instance)
            {
                if (__instance is NeutronicTransmutationChamber)
                {
                    BuildingDef buildingDef = __instance.gameObject.GetComponent<Building>().Def;
                    buildingDef.EnergyConsumptionWhenActive = (__instance.CurrentWorkingOrder as NeutronicTransmutationRecipe).energyRequired;

                    __instance.gameObject.GetComponent<EnergyConsumer>().BaseWattageRating = (__instance.CurrentWorkingOrder as NeutronicTransmutationRecipe).energyRequired;
                }
            }
        }

        //// Add disease to the output
        [HarmonyPatch(typeof(ComplexFabricator))]
        [HarmonyPatch("SpawnOrderProduct")]
        public static class ComplexFabricator_SpawnOrderProduct_Patch
        {
            public static void Postfix(ComplexFabricator __instance, List<GameObject> __result, ComplexRecipe recipe)
            {
                if (recipe is NeutronicTransmutationRecipe colliderRecipe)
                {
                    foreach (GameObject recipeResult in __result)
                    {
                        PrimaryElement primaryElement = recipeResult.GetComponent<PrimaryElement>();
                        primaryElement.SetUseSimDiseaseInfo(false);
                        primaryElement.AddDisease(Db.Get().Diseases.GetIndex(RadiationPoisoning.ID), colliderRecipe.radiationCreated, "NeutronicTransmutation.Emit");
                    }
                }
            }
        }
    }
}
