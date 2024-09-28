using UnityEngine;
using static GigaWattWire.WirePatchs;

namespace GigaWattWire
{
    public class JacketedWireBridgeConfig : WireBridgeConfig
    {
        public new const string ID = "JacketedWireBridge";

        protected override string GetID() => ID;
        
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = base.CreateBuildingDef();
            buildingDef.AnimFiles = new KAnimFile[1] { Assets.GetAnim("jacketed_wire_bridge_kanim") };
            buildingDef.Mass = WirePatchs.JACKETED_WIRE_MASS_KG;
            buildingDef.MaterialCategory = WirePatchs.MEGAWATT_WIRE_MATERIALS;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, ID);

            // Insulated option
            if (GameOnLoadPatch.Settings.MakeWireBridgeInsulated) buildingDef.ThermalConductivity = 0.01f;

            return buildingDef;
        }

        protected override WireUtilityNetworkLink AddNetworkLink(GameObject go)
        {
            WireUtilityNetworkLink wireUtilityNetworkLink = base.AddNetworkLink(go);
            wireUtilityNetworkLink.maxWattageRating = WirePatchs.WattageRating.Max5kW.ToWireWattageRating();
            return wireUtilityNetworkLink;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            Constructable component = go.GetComponent<Constructable>();
            component.requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
        }
        
    }

}
