using HarmonyLib;
using Klei.AI;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using Pholib;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HighTechIndustry
{
    public class HighTechMod : UserMod2
    {
        public static HighTechOption Settings = null;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(HighTechOption));
            ReadSettings();

            new PLocalization().Register();
            Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));
        }

        public static void ReadSettings()
        {
            Settings = POptions.ReadSettings<HighTechOption>();
            if (Settings == null)
            {
                Settings = new HighTechOption();
            }
        }
    }

    // Add the extractor to tech tree 
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DatabaseAddingPatch
    {
        public static void Postfix()
        {
            Utilities.AddBuildingTech("NuclearRefinement", NeutronicTransmutationChamberConfig.ID);
        }
    }

    [HarmonyPatch(typeof(GeneratedBuildings))]
    [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    public static class ImplementationPatch
    {
        private static void Prefix()
        {
            Utilities.AddBuilding("HEP", NeutronicTransmutationChamberConfig.ID,
                PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.NAME,
                PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.DESC,
                PHO_STRINGS.NEUTRONIC_TRANSMUTATION_CHAMBER.EFFECT,
                TUNING.BUILDINGS.PlanSubcategoryName.producers,
                UraniumCentrifugeConfig.ID);
        }
    }


    // Patch Helium wrong values and enable it
    [HarmonyPatch(typeof(ElementLoader))]
    [HarmonyPatch("CopyEntryToElement")]
    public static class HighTechIndustry_ElementLoader_PatchHelium
    {
        public static void Postfix(ElementData.ElementEntry entry, Element elem)
        {
            if (HighTechMod.Settings.RestoreHeliumTrueProperty && elem.id == SimHashes.Helium)
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
                StringBuilder stringBuilder = new StringBuilder();
                if (selectedRecipe.time >= 600)
                {
                    stringBuilder.Append(selectedRecipe.time / 600);
                    stringBuilder.Append(" ");
                    stringBuilder.Append(UI.RETIRED_COLONY_INFO_SCREEN.CYCLES.ToString().ToLower());
                }
                else
                {
                    stringBuilder.Append(selectedRecipe.time);
                    stringBuilder.Append(" ");
                    stringBuilder.Append(UI.UNITSUFFIXES.SECONDS.ToString().ToLower());
                }
                stringBuilder.Append("\t");
                stringBuilder.Append(Utilities.FormatColored(colliderRecipe.energyRequired + " " + UI.UNITSUFFIXES.ELECTRICAL.WATT.ToString(), "c08210"));

                __instance.recipeDuration.SetText(stringBuilder.ToString());
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
                    // primaryElement.SetUseSimDiseaseInfo(false);
                    primaryElement.AddDisease(Db.Get().Diseases.GetIndex(RadiationPoisoning.ID), colliderRecipe.radiationCreated, "NeutronicTransmutation.Emit");
                }
            }

            // If that's the new recipe of the centrifuger, add nuclear disease
            if (__instance is UraniumCentrifuge && recipe.ingredients.Any(recipeElement => recipeElement.material == SimHashes.NuclearWaste.CreateTag()))
            {
                foreach (GameObject recipeResult in __result)
                {
                    PrimaryElement primaryElement = recipeResult.GetComponent<PrimaryElement>();
                    // primaryElement.SetUseSimDiseaseInfo(false);
                    primaryElement.AddDisease(Db.Get().Diseases.GetIndex(RadiationPoisoning.ID), 20000, "UraniumCentrifuge.Emit");
                }
            }
        }
    }
}
