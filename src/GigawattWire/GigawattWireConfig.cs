using HarmonyLib;
using UnityEngine;

namespace Egladil
{
    public class GigawattWireConfig : BaseWireConfig
    {
        public const string ID = "GigawattWire";
        
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = CreateBuildingDef(ID, "gigawatt_wire_kanim", 3f, WirePatchs.GIGAWATT_WIRE_MASS_KG, 0.05f, noise: TUNING.NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER3);
            buildingDef.MaterialCategory = WirePatchs.GIGAWATT_WIRE_MATERIALS;
            buildingDef.Overheatable = true;
            buildingDef.OverheatTemperature = 3.15f;
            buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            DoPostConfigureComplete(WirePatchs.WattageRating.Max1GW.ToWireWattageRating(), go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            Constructable component = go.GetComponent<Constructable>();
            component.requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
        }
    }
}
