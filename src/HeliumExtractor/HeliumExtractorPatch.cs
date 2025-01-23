using HarmonyLib;
using Pholib;
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
            private static void Prefix()
            {
                Utilities.AddBuilding("Refining", HeliumExtractorConfig.ID, PHO_STRINGS.HELIUMEXTRACTOR.NAME, PHO_STRINGS.HELIUMEXTRACTOR.DESC, PHO_STRINGS.HELIUMEXTRACTOR.EFFECT);
            }

        }
        // Add the extractor to tech tree 
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class DatabaseAddingPatch
        {
            public static void Postfix()
            {
                Utilities.AddBuildingTech("HighTempForging", HeliumExtractorConfig.ID);
                Strings.Add("STRINGS.MISC.TAGS.COMBUSTIBLEGAS", PHO_STRINGS.COMBUSTIBLEGAS);
            }
        }

        //public static readonly Tag ConductorTag = TagManager.Create(COMBUSTIBLEGAS, PHO_STRINGS.COMBUSTIBLEGAS);

        [HarmonyPatch(typeof(ElementLoader), "CopyEntryToElement")]
        public class PropaneCombustibleAdder
        {
            public static void Postfix(Element elem)
            {
                // Enable helium and propane
                if (elem.id == SimHashes.Helium || elem.id == SimHashes.LiquidHelium || elem.id == SimHashes.Propane || elem.id == SimHashes.LiquidPropane || elem.id == SimHashes.SolidPropane)
                {
                    elem.disabled = false;
                    elem.oreTags = elem.oreTags.Except(new Tag[] { GameTags.HideFromCodex, GameTags.HideFromSpawnTool }).Cast<Tag>().ToArray(); // Remove Propane Hide tags
                }

                // Add CombustibleGas tag to Propane
                if (elem.id == SimHashes.Propane && !elem.oreTags.Contains(GameTags.CombustibleGas))
                {
                    IEnumerable list = elem.oreTags.AddItem(GameTags.CombustibleGas);
                    elem.oreTags = list.Cast<Tag>().ToArray();
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
                energyGenerator.formula.inputs = new EnergyGenerator.InputItem[]
                    {
                new EnergyGenerator.InputItem(GameTags.CombustibleGas, 0.09f, 0.90000004f)
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
        // Change the FoodDehydratorConfig fuel tag from Methane to CombustibleGas
        [HarmonyPatch(typeof(FoodDehydratorConfig), nameof(FoodDehydratorConfig.ConfigureBuildingTemplate))]
        public class FoodDehydratorConfig_Propane_Patch
        {
            public static void Prefix(FoodDehydratorConfig __instance, GameObject go)
            {
                FOODDEHYDRATORTUNING.FUEL_TAG = GameTags.CombustibleGas;
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
                            if (!component2.HasTag(__instance.keepAdditionalTag) && component2 && !hashSet.Contains(component2.PrefabID())) // My change here 
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
