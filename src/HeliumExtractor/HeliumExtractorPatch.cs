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
