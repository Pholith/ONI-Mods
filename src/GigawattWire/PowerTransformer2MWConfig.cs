using UnityEngine;

namespace Egladil
{
    public class PowerTransformer2MWConfig : IBuildingConfig
    {
        public const string ID = "PowerTransformer2MW";

        public override BuildingDef CreateBuildingDef()
        {
            int width = 4;
            int height = 3;
            int hitpoints = 30;
            float construction_time = 30f;
            float[] massKg = new float[] { TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0], TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0] };
            float melting_point = 800f;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, "transformer_kanim", hitpoints, construction_time, massKg, WirePatchs.MEGAWATT_WIRE_MATERIALS, melting_point, BuildLocationRule.OnFloor, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, TUNING.NOISE_POLLUTION.NOISY.TIER5);
            buildingDef.RequiresPowerInput = true;
            buildingDef.UseWhitePowerOutputConnectorColour = true;
            buildingDef.PowerInputOffset = new CellOffset(-1, 2);
            buildingDef.PowerOutputOffset = new CellOffset(2, 1);
            buildingDef.ElectricalArrowOffset = new CellOffset(2, 1);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.ExhaustKilowattsWhenActive = 0f;
            buildingDef.SelfHeatKilowattsWhenActive = 1f;
            buildingDef.Entombable = true;
            buildingDef.GeneratorWattageRating = 2e6f;
            buildingDef.GeneratorBaseCapacity = 2e6f;
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddComponent<RequireInputs>();
            BuildingDef def = go.GetComponent<Building>().Def;
            Battery battery = go.AddOrGet<Battery>();
            battery.powerSortOrder = 1000;
            battery.capacity = def.GeneratorWattageRating;
            battery.chargeWattage = def.GeneratorWattageRating;
            PowerTransformer powerTransformer = go.AddComponent<PowerTransformer>();
            powerTransformer.powerDistributionOrder = 9;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Object.DestroyImmediate(go.GetComponent<EnergyConsumer>());
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}
