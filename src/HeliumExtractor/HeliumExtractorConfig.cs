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
            string anim = "helium_extractor_kanim";
            int hitpoints = 30;
            float construction_time = 30f;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            string[] all_METALS = MATERIALS.ALL_METALS;
            float melting_point = 800f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 420f;
            buildingDef.SelfHeatKilowattsWhenActive = 12f;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OutputConduitType = ConduitType.Gas;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
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
            heliumExtractor.overpressureMass = 10f;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = totalConversion;
            conduitConsumer.capacityTag = SimHashes.Methane.CreateTag();
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.capacityKG = 10f;
            conduitConsumer.forceAlwaysSatisfied = true;

            
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.blocked = true;
            conduitDispenser.invertElementFilter = true;
            conduitDispenser.elementFilter = new SimHashes[]
            {
                SimHashes.Methane,
                SimHashes.Propane
            };

            ConduitDispenser conduitDispenser2 = go.AddComponent<ConduitDispenser>();
            conduitDispenser2.conduitType = ConduitType.Gas;
            conduitDispenser2.invertElementFilter = true;
            conduitDispenser2.elementFilter = new SimHashes[]
            {
                SimHashes.Methane,
                SimHashes.Helium
            };
            conduitDispenser2.useSecondaryOutput = true;


            ConduitSecondaryOutput secondOutput = go.AddOrGet<ConduitSecondaryOutput>();
            secondOutput.portInfo = secondaryPort;

            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.capacityKg = 10;

            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
            {
                new ElementConverter.ConsumedElement(SimHashes.Methane.CreateTag(), totalConversion)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(heliumConversionRate, SimHashes.Sulfur, 350.15f, false, false),
                new ElementConverter.OutputElement(heliumConversionRate, SimHashes.Helium, 80.15f, false, true),
                new ElementConverter.OutputElement(propaneConversionRate, SimHashes.Propane, 81.15f, false, true)
            };
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            AttachPort(go);
        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            AttachPort(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            AttachPort(go);
        }

        private void AttachPort(GameObject go)
        {
            ConduitSecondaryOutput secondOutput = go.AddOrGet<ConduitSecondaryOutput>();
            secondOutput.portInfo = secondaryPort;
        }



        private readonly ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-1, 2));

        public static float totalConversion = 0.5f; // 500 g (input of pipes)
        public static float heliumConversionRate = 0.05f / 2; // 5% of 1kg
        public static float sulfureConversionRate = 0.05f / 2; // 5% of 1kg
        public static float propaneConversionRate = totalConversion - heliumConversionRate - sulfureConversionRate; // the rest


        public const string ID = "HeliumExtractor";
    }
}
