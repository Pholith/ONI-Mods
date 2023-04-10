using Database;
using HarmonyLib;
using Pholib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeliumExtractor
{
    internal class HeliumExtractorPatch
    {

        // Add strings and add the extractor to plan screen
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class ImplementationPatch
        {
            public static LocString NAME = new LocString("Helium Extractor",
                "STRINGS.BUILDINGS.PREFABS." + HeliumExtractorConfig.ID.ToUpper() + ".NAME");

            public static LocString DESC = new LocString("Helium can only be produced from the extration of Natural gas.",
                "STRINGS.BUILDINGS.PREFABS." + HeliumExtractorConfig.ID.ToUpper() + ".DESC");

            public static LocString EFFECT = new LocString("Transforms "
                + STRINGS.UI.FormatAsLink("natural gas", "METHANE")
                + " into "
                + STRINGS.UI.FormatAsLink("helium", "HELIUM") + ", "
                + STRINGS.UI.FormatAsLink("propane", "PROPANE") + " and "
                + STRINGS.UI.FormatAsLink("sulfur", "SULFUR") + "."
                + "\n\n" + STRINGS.UI.FormatAsLink("Helium", "HELIUM") + " is a useful gas with interesting physical properties.\nPropane can be used the same way than Natural gas.",
                "STRINGS.BUILDINGS.PREFABS." + HeliumExtractorConfig.ID.ToUpper() + ".EFFECT");

            private static void Prefix()
            {
                Strings.Add(NAME.key.String, NAME.text);
                Strings.Add(DESC.key.String, DESC.text);
                Strings.Add(EFFECT.key.String, EFFECT.text);
                ModUtil.AddBuildingToPlanScreen("Refining", HeliumExtractorConfig.ID);
            }

        }
        // Add the extractor to tech tree 
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class DatabaseAddingPatch
        {
            public static void Postfix()
            {
                Utilities.AddBuildingTech("HighTempForging", HeliumExtractorConfig.ID);
            }
        }

        // Add CombustibleGas tag to Propane
        [HarmonyPatch(typeof(ElementLoader), "CopyEntryToElement")]
        public class PropaneCombustibleAdder
        {
            public static void Postfix(Element elem)
            {
                if (elem.id == SimHashes.Propane)
                {
                    IEnumerable list = elem.oreTags.AddItem(GameTags.CombustibleGas);
                    elem.oreTags = list.Cast<Tag>().ToArray();
                }
            }
        }
        // Enable helium and propane
        [HarmonyPatch(typeof(ElementLoader), "CopyEntryToElement")]
        public class HeliumEnablePatch
        {
            public static void Postfix(Element elem)
            {
                if (elem.id == SimHashes.Helium || elem.id == SimHashes.LiquidHelium || elem.id == SimHashes.Propane || elem.id == SimHashes.LiquidPropane || elem.id == SimHashes.SolidPropane)
                {
                    elem.disabled = false;
                }
            }
        }
    
        // Add propane in the MethaneGeneratorConfig element list
        [HarmonyPatch(typeof(MethaneGeneratorConfig), "DoPostConfigureComplete")]
        public class MethaneGeneratorPropanePatch
        {
            public static void Postfix(GameObject go)
            {
                EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
                energyGenerator.formula = new EnergyGenerator.Formula
                {
                    inputs = new EnergyGenerator.InputItem[]
                    {
                new EnergyGenerator.InputItem(GameTags.CombustibleGas, 0.09f, 0.90000004f)
                    },
                    outputs = new EnergyGenerator.OutputItem[]
                    {
                new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.0675f, false, new CellOffset(1, 1), 313.15f),
                new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.0225f, true, new CellOffset(0, 2), 383.15f)
                    }
                };


                ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
                conduitDispenser.elementFilter = conduitDispenser.elementFilter.AddItem(SimHashes.Propane).ToArray();

            }
        }
        // Change the GourmetCookingStationConfig fuel tag from Methane to CombustibleGas
        [HarmonyPatch(typeof(GourmetCookingStationConfig), nameof(GourmetCookingStationConfig.ConfigureBuildingTemplate))]
        public class GourmetCookingStationPropanePatch
        {
            public static void Prefix(GourmetCookingStationConfig __instance, GameObject go)
            {
                Traverse.Create(__instance).Field("FUEL_TAG").SetValue(GameTags.CombustibleGas);
            }
        }

        [HarmonyPatch(typeof(ComplexFabricator), "DropExcessIngredients")]
        public class GourmetCookingStationPropanePatch2
        {
            public static bool Prefix(ComplexFabricator __instance, Storage storage)
            {

                var recipe_list = Traverse.Create(__instance).Field<ComplexRecipe[]>("recipe_list").Value;

                HashSet<Tag> hashSet = new HashSet<Tag>();
                if (__instance.keepAdditionalTag != Tag.Invalid)
                {
                    hashSet.Add(__instance.keepAdditionalTag);
                }
                for (int i = 0; i < recipe_list.Length; i++)
                {
                    ComplexRecipe complexRecipe = recipe_list[i];
                    if (__instance.IsRecipeQueued(complexRecipe))
                    {
                        foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
                        {
                            hashSet.Add(recipeElement.material);
                        }
                    }
                }
                for (int k = storage.items.Count - 1; k >= 0; k--)
                {
                    GameObject gameObject = storage.items[k];
                    if (!(gameObject == null))
                    {
                        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
                        if (!(component == null) && (!__instance.keepExcessLiquids || !component.Element.IsLiquid))
                        {
                            KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
                            if (!(component2.HasTag(__instance.keepAdditionalTag)) && component2 && !hashSet.Contains(component2.PrefabID())) // My change here 
                            {
                                storage.Drop(gameObject, true);
                            }
                        }
                    }
                }
                return false;
            }
        }
    }
}
