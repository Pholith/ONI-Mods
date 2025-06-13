using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace High_Pressure_Applications.BuildingConfigs
{
    public class DecompressionGasValveConfig : IBuildingConfig
    {
        public const string Id = "DecompressionGasValve";
        public const string DisplayName = "Decompression Gas Valve";
        public const string Description = "A mechanical valve capable of reducing the flow of gas from a pressurized pipe to a normal pipe, avoid it to break.";
        public static string Effect = string.Concat(new string[]
                {
                    "Allows ",
                    UI.FormatAsLink("Gases", "ELEMENTS_GAS"),
                    " to be transfered from ",
                    UI.FormatAsLink("High Pressure Gas Pipe", HighPressureGasConduitConfig.Id),
                    " to normal ",
                    UI.FormatAsLink("Pipes", "GASCONDUIT"),
                    "."
                });

        public static readonly List<Storage.StoredItemModifier> ValveStoredItemModifiers;
        static DecompressionGasValveConfig()
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
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(Id, 2, 1, "deco_gas_valve_kanim", 100, 50f, quantity1, materials1, 800f, BuildLocationRule.Anywhere, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OutputConduitType = ConduitType.Gas;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.PermittedRotations = PermittedRotations.R360;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, buildingDef.PrefabID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<Reservoir>();
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showDescriptor = false;
            storage.storageFilters = STORAGEFILTERS.GASES;
            storage.capacityKg = 10f;
            storage.SetDefaultStoredItemModifiers(ValveStoredItemModifiers);
            storage.showCapacityStatusItem = false;
            storage.showCapacityAsMainStatus = false;

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = storage.capacityKg;

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.elementFilter = null;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
        }
    }
}
