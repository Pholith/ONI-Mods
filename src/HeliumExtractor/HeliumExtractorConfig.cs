using TUNING;
using UnityEngine;

namespace HeliumExtractor
{

    public class HeliumExtractorConfig : IBuildingConfig
    {

        // Buildingdef from AlgaeDistillery example
        public override BuildingDef CreateBuildingDef()
        {
            string id = "HeliumExtractor";
            int width = 3;
            int height = 4;
            string anim = "algae_distillery_kanim";
            int hitpoints = 30;
            float construction_time = 60f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(1, 0);
            buildingDef.EnergyConsumptionWhenActive = 1200f;
            buildingDef.SelfHeatKilowattsWhenActive = 24f;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OutputConduitType = ConduitType.Gas;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.ModifiesTemperature = false;
            return buildingDef;
        }

        //ConfigureBuildingTemplate from rafinnery example
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
            HeliumExtractor heliumExtractor = go.AddOrGet<HeliumExtractor>();
            heliumExtractor.overpressureMass = 5f;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = totalConversion;
            conduitConsumer.capacityTag = SimHashes.Methane.CreateTag();
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.capacityKG = 10f;
            conduitConsumer.forceAlwaysSatisfied = true;
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.invertElementFilter = true;
            conduitDispenser.elementFilter = new SimHashes[]
            {
                SimHashes.Methane
            };
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
            {
                new ElementConverter.ConsumedElement(SimHashes.Methane.CreateTag(), totalConversion)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(heliumConversionRate, SimHashes.Sulfur, 350.15f, false, false),
                new ElementConverter.OutputElement(heliumConversionRate, SimHashes.Helium, 80.15f, false, true),
                new ElementConverter.OutputElement(propaneConverssionRate, SimHashes.LiquidPropane, 80.15f, false, false, 1f, 1f)
            };
            Prioritizable.AddRef(go);
        }

        public static float totalConversion = 0.5f; // 500 g (input of pipes)
        public static float heliumConversionRate = 0.05f / 2; // 5% of 1kg
        public static float sulfureConverssionRate = 0.05f / 2; // 5% of 1kg
        public static float propaneConverssionRate = totalConversion - heliumConversionRate - sulfureConverssionRate; // the rest

        public override void DoPostConfigureComplete(GameObject go)
        {
        }

        public const string ID = "HeliumExtractor";
    }
}
