using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace High_Pressure_Applications.BuildingConfigs
{
    public class DecompressionLiquidValveConfig : IBuildingConfig
    {
        public const string Id = "DecompressionLiquidValve";
        public const string DisplayName = "Decompression Liquid Valve";
        public const string Description = "A mechanical valve capable of reducing the flow of liquid from a pressurized pipe to a normal pipe, avoid it to break.";
        public static string Effect = string.Concat(new string[]
                {
                    "Allows ",
                    UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
                    " to be transfered from ",
                    UI.FormatAsLink("High Pressure Liquid Pipe", HighPressureLiquidConduitConfig.Id),
                    " to normal ",
                    UI.FormatAsLink("Pipes", "LIQUIDCONDUIT"),
                    "."
                });

        public static readonly List<Storage.StoredItemModifier> ValveStoredItemModifiers;
        static DecompressionLiquidValveConfig()
        {
            List<Storage.StoredItemModifier> list1 = new List<Storage.StoredItemModifier>();
            list1.Add(Storage.StoredItemModifier.Hide);
            list1.Add(Storage.StoredItemModifier.Seal);
            list1.Add(Storage.StoredItemModifier.Insulate);
            ValveStoredItemModifiers = list1;
        }

        public override BuildingDef CreateBuildingDef()
        {
            float[] quantity1 = new float[] { 50f, 20f };
            string[] materials1 = new string[] { SimHashes.Steel.ToString(), SimHashes.Polypropylene.ToString() };
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(Id, 2, 1, "deco_liquid_valve_kanim", 100, 50f, quantity1, materials1, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.PermittedRotations = PermittedRotations.R360;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, Id);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<Reservoir>();
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showDescriptor = false;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.capacityKg = 40f;
            storage.SetDefaultStoredItemModifiers(ValveStoredItemModifiers);
            storage.showCapacityStatusItem = false;
            storage.showCapacityAsMainStatus = false;

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = storage.capacityKg;

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.elementFilter = null;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
        }
    }
}
