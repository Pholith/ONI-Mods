using HarmonyLib;
using Pholib;
using System.Collections.Generic;
using UnityEngine;

namespace HighTechIndustry
{

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

        // Helium disabled in game but reabled by HeliumExtractor and this mod is the option is enabled.
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
                }, ChemicalRefineryConfig.ID, 60, PHO_STRINGS.RECIPE.POWER_TO_METHANE_DESC, ComplexRecipe.RecipeNameDisplay.Custom, 200);
            powerToGas.customName = PHO_STRINGS.RECIPE.POWER_TO_METHANE_NAME;
            powerToGas.customSpritePrefabID = ElementLoader.FindElementByHash(SimHashes.Methane).id.ToString();
            powerToGas.requiredTech = Db.Get().TechItems.disposableElectrobankUraniumOre.parentTechId;
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


            float recipeAmount = 200;
            var results = new ComplexRecipe.RecipeElement[] {
                    new ComplexRecipe.RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.uranium.id.CreateTag(), recipeAmount / 100f / 2f), // 1kg of enriched uranium gives 100 kg of waste
                    //new ComplexRecipe.RecipeElement(HighTechIndustry_NuclearWasteRecyclePatch.depletedUranium.id.CreateTag(), 50f / 100f * recipeAmount), // 1kg of enriched uranium gives 100 kg of waste
                    new ComplexRecipe.RecipeElement(SimHashes.MoltenUranium.CreateTag(), 50f / 100f * recipeAmount), // 1kg of enriched uranium gives 100 kg of waste
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
                    120f, PHO_STRINGS.RECIPE.NUCLEAR_WASTE_RECYCLING_DESC, ComplexRecipe.RecipeNameDisplay.Custom, 200);
                recipe.customName = PHO_STRINGS.RECIPE.NUCLEAR_WASTE_RECYCLING_NAME;
                recipe.requiredTech = "NuclearPropulsion";
                recipe.customSpritePrefabID = SimHashes.NuclearWaste.CreateTag().ToString();
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

            // Make the centrifuge sealed.
            ___storedItemModifiers.Add(Storage.StoredItemModifier.Seal);
            go.AddTag(GameTags.CorrosionProof);

        }
        private static void Postfix(GameObject go)
        {
            // Make it accept nuclear waste
            UraniumCentrifuge uraniumCentrifuge = go.GetComponent<UraniumCentrifuge>();

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.capacityTag = SimHashes.NuclearWaste.CreateTag();
            conduitConsumer.storage = uraniumCentrifuge.inStorage;
        }
    }

    [HarmonyPatch(typeof(UraniumCentrifugeConfig))]
    [HarmonyPatch(nameof(UraniumCentrifugeConfig.DoPostConfigureComplete))]
    public static class HighTechIndustry_DoPostConfigureComplete_WarningPatch
    {
        private static void Postfix(GameObject go)
        {
            // Remove the not connected pipe warning
            go.GetComponent<RequireInputs>().SetRequirements(true, false);
        }
    }

    [HarmonyPatch(typeof(UraniumCentrifugeConfig))]
    [HarmonyPatch(nameof(UraniumCentrifugeConfig.CreateBuildingDef))]
    public static class HighTechIndustry_UraniumCentrifugeConfig_CreateBuildingDef
    {
        private static void Postfix(BuildingDef __result)
        {
            // Add radiation source icon for centrifuger
            __result.DiseaseCellVisName = "RadiationSickness";

            // Make it accept nuclear waste
            __result.InputConduitType = ConduitType.Liquid;
            __result.UtilityInputOffset = new CellOffset(-1, 3);
        }
    }


}
