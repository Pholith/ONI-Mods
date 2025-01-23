using Pholib;
using System.Collections.Generic;
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
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = GameOnLoadPatch.Settings.PowerConsumption;
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

            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.capacityKg = 10;

            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.consumptionRate = totalConversion;
            conduitConsumer.capacityTag = SimHashes.Methane.CreateTag();
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
            conduitConsumer.capacityKG = 10f;
            conduitConsumer.forceAlwaysSatisfied = true;

            DropAllWorkable dropAllWorkable = go.AddOrGet<DropAllWorkable>();
            dropAllWorkable.removeTags = new List<Tag>
        {
            GameTags.GasSource, SimHashes.Sulfur.CreateTag(),
        };
            dropAllWorkable.resetTargetWorkableOnCompleteWork = true;


            // Automatic patch
            var buildingComplete = go.AddOrGet<BuildingComplete>();
            if (GameOnLoadPatch.Settings.DisableDuplicantOperation)
            {
                buildingComplete.isManuallyOperated = false;
                go.AddOrGet<WaterPurifier>();
                conduitConsumer.capacityKG = 1f;
                storage.capacityKg = 1;
            }
            else
            {
                buildingComplete.isManuallyOperated = true;
                HeliumExtractor heliumExtractor = go.AddOrGet<HeliumExtractor>();
                heliumExtractor.overpressureMass = 15f;
            }

            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Gas;
            conduitDispenser.blocked = true;
            conduitDispenser.elementFilter = new SimHashes[]
            {
                SimHashes.Helium,
            };

            ConduitDispenser conduitDispenser2 = go.AddComponent<ConduitDispenser>();
            conduitDispenser2.conduitType = ConduitType.Gas;
            conduitDispenser2.elementFilter = new SimHashes[]
            {
                SimHashes.Propane,
            };
            conduitDispenser2.useSecondaryOutput = true;


            ConduitSecondaryOutput secondOutput = go.AddOrGet<ConduitSecondaryOutput>();
            secondOutput.portInfo = secondaryPort;


            ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
            elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
            {
                new ElementConverter.ConsumedElement(SimHashes.Methane.CreateTag(), totalConversion)
            };
            elementConverter.outputElements = new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(sulfureConversionRate, SimHashes.Sulfur, 60f.CelciusToKelvin(), false, false),
                new ElementConverter.OutputElement(heliumConversionRate, SimHashes.Helium, (-80f).CelciusToKelvin(), false, true),
                new ElementConverter.OutputElement(propaneConversionRate, SimHashes.Propane, (-80f).CelciusToKelvin(), false, true)
            };
            Prioritizable.AddRef(go);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            AttachPort(go);
            /*HeliumExtractor heliumExtractor = go.AddOrGet<HeliumExtractor>();
            heliumExtractor.overpressureMass = 15f;
            Logs.Log("test 4");
            Logs.Log(heliumExtractor?.smi?.sm.GetState("root.ready").transitions.Dump());
            Logs.Log(heliumExtractor?.smi?.sm.GetState("root.ready").enterActions.Dump());
            Logs.Log(heliumExtractor?.smi?.sm.masterTarget.GetType());
            //heliumExtractor.smi.sm.GetState("root.ready").enterActions[1] = new OilRefinery.States.Action("", (object) new OilRefinery.States.WorkingState.Callback((smi) =>Traverse.Create(heliumExtractor).Field<Operational>("operational").Value.SetActive(true)));
            Logs.Log(heliumExtractor?.smi?.sm.GetState("root.ready").enterActions.Dump());*/

        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            AttachPort(go);
        }

        public void Test()
        {

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

        public static float totalConversion = GameOnLoadPatch.Settings.MethaneConversionPerSeconds; // 500 g (input of pipes)
        public static float heliumConversionRate = 0.05f * totalConversion; // 5% of 1kg
        public static float sulfureConversionRate = 0.05f * totalConversion; // 5% of 1kg
        public static float propaneConversionRate = totalConversion - heliumConversionRate - sulfureConversionRate; // the rest


        public const string ID = "HeliumExtractor";
    }
}
