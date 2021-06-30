using Database;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeliumExtractor
{
    internal class HeliumExtractorPatch
    {
        //Change the color of the extractor
        [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
        public class ColorPatch
        {
            public static void Postfix(BuildingComplete __instance)
            {
                KAnimControllerBase kAnimBase = __instance.GetComponent<KAnimControllerBase>();
                if (kAnimBase != null)
                {
                    if (__instance.name == "HeliumExtractorComplete")
                    {
                        float r = 255;
                        float g = 255 - 200;
                        float b = 255 - 100;
                        kAnimBase.TintColour = new Color(r / 255f, g / 255f, b / 255f);
                    }
                }
            }
        }

        // Add strings and add the extractor to plan screen
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch("LoadGeneratedBuildings")]
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
                + STRINGS.UI.FormatAsLink("propane", "LIQUIDPROPANE") + " and "
                + STRINGS.UI.FormatAsLink("sulfur", "SULFUR") + "."
                + "\n\n" + STRINGS.UI.FormatAsLink("Helium", "HELIUM") + " is a useful gas with interesting physical properties.\n",
                "STRINGS.BUILDINGS.PREFABS." + HeliumExtractorConfig.ID.ToUpper() + ".EFFECT");

            private static void Prefix()
            {
                Strings.Add(NAME.key.String, NAME.text);
                Strings.Add(DESC.key.String, DESC.text);
                Strings.Add(EFFECT.key.String, EFFECT.text);
                ModUtil.AddBuildingToPlanScreen("Refining", HeliumExtractorConfig.ID);
            }

            private static void Postfix()
            {
                object obj = Activator.CreateInstance(typeof(HeliumExtractorConfig));
                BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
            }

        }
        // Add the extractor to tech tree 
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class DatabaseAddingPatch
        {
            public static void Prefix()
            {
                List<string> ls = new List<string>(Techs.TECH_GROUPING["HighTempForging"]) { HeliumExtractorConfig.ID };
                Techs.TECH_GROUPING["HighTempForging"] = ls.ToArray();
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
                    IEnumerable list = elem.oreTags.Add(GameTags.CombustibleGas);
                    elem.oreTags = list.Cast<Tag>().ToArray();
                }
            }
        }
        // Enable helium
        [HarmonyPatch(typeof(ElementLoader), "CopyEntryToElement")]
        public class HeliumEnablePatch
        {
            public static void Postfix(Element elem)
            {
                if (elem.id == SimHashes.Helium || elem.id == SimHashes.LiquidHelium)
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
                ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
                conduitDispenser.elementFilter = conduitDispenser.elementFilter.Add(SimHashes.Propane).ToArray();

            }
        }
        // Change the GourmetCookingStationConfig fuel tag from Methane to CombustibleGas
        [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureBuildingTemplate")]
        public class GourmetCookingStationPropanePatch
        {
            public static void Prefix(GourmetCookingStationConfig __instance)
            {
                Traverse.Create(__instance).Field("FUEL_TAG").SetValue(new Tag(GameTags.CombustibleGas));
            }
        }
    }
}
