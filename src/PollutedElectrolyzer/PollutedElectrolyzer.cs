﻿using HarmonyLib;
using Pholib;
using TUNING;
using UnityEngine;

namespace PollutedElectrolyzer
{

    public class PollutedElectrolyserConfig : IBuildingConfig
    {
        // copy paste with some change
        public override BuildingDef CreateBuildingDef()
        {
            string id = "PollutedElectrolyzer";
            int width = 2;
            int height = 2;
            string anim = "electrolyzer_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(1, 0);
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.ExhaustKilowattsWhenActive = 0.25f;
            buildingDef.SelfHeatKilowattsWhenActive = 1f;
            buildingDef.ViewMode = OverlayModes.Oxygen.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));

            return buildingDef;
        }
        // Big copy paste of the game code
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
            electrolyzer.maxMass = 1.8f;
            electrolyzer.hasMeter = true;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 1f;
            conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 2f;
            storage.showInUI = true;
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
            {
                new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 1f)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(0.888f, SimHashes.ContaminatedOxygen, OXYGEN_TEMPERATURE),
                new ElementConverter.OutputElement(0.111999989f, SimHashes.Hydrogen, OXYGEN_TEMPERATURE)
            };
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
        public const string ID = "PollutedElectrolyzer";

        public const float WATER2OXYGEN_RATIO = 0.888f;
        public const float OXYGEN_TEMPERATURE = 313.15f; // 343.15f original
    }


    //ColorPatch
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class ColorPatch
    {
        public static void Postfix(BuildingComplete __instance)
        {
            KAnimControllerBase kAnimBase = __instance.GetComponent<KAnimControllerBase>();
            if (kAnimBase != null)
            {
                if (__instance.name == "PollutedElectrolyzerComplete") // I found it in the logs
                {
                    float r = 255 - 70;// 100
                    float g = 255; // more the int after - is big, less green and more pink
                    float b = 255 - 150; //50
                    kAnimBase.TintColour = new Color(r / 255f, g / 255f, b / 255f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public class PollutedElectrolyzerPatch
    {
        public static LocString NAME = new LocString("Polluted Electrolyzer",
            "STRINGS.BUILDINGS.PREFABS." + PollutedElectrolyserConfig.ID.ToUpper() + ".NAME");

        public static LocString DESC = new LocString("Water enters on one side, oxygen essential to life exits on the other.",
            "STRINGS.BUILDINGS.PREFABS." + PollutedElectrolyserConfig.ID.ToUpper() + ".DESC");

        public static LocString EFFECT = new LocString("Transforms " + STRINGS.UI.FormatAsLink("Dirty water", "DIRTYWATER") + " into " + STRINGS.UI.FormatAsLink("Polluted oxygen", "CONTAMINATEDOXYGEN") + " and " + STRINGS.UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".",
            "STRINGS.BUILDINGS.PREFABS." + PollutedElectrolyserConfig.ID.ToUpper() + ".EFFECT");

        private static void Prefix()
        {
            Strings.Add(NAME.key.String, NAME.text);
            Strings.Add(DESC.key.String, DESC.text);
            Strings.Add(EFFECT.key.String, EFFECT.text);
            ModUtil.AddBuildingToPlanScreen("Oxygen", PollutedElectrolyserConfig.ID);
        }
    }
    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DbPatch
    {
        public static void Postfix()
        {
            Utilities.AddBuildingTech("ImprovedOxygen", PollutedElectrolyserConfig.ID);
        }
    }

}
