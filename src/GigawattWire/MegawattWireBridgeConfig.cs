﻿using UnityEngine;
using static GigaWattWire.WirePatchs;

namespace GigaWattWire
{
    public class MegawattWireBridgeConfig : WireBridgeHighWattageConfig
    {
        public new const string ID = "MegawattWireBridge";

        protected override string GetID() => ID;

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = base.CreateBuildingDef();
            buildingDef.AnimFiles = new KAnimFile[1] { Assets.GetAnim("megawatt_wire_bridge_kanim") };
            buildingDef.Mass = WirePatchs.MEGAWATT_WIRE_MASS_KG;
            buildingDef.MaterialCategory = WirePatchs.MEGAWATT_WIRE_MATERIALS;
            buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
            buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, ID);

            // Insulated option
            if (GameOnLoadPatch.Settings.MakeWireBridgeInsulated) buildingDef.ThermalConductivity = 0.01f;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);
            if (GameOnLoadPatch.Settings.MakeWireBridgeInsulated) go.AddOrGet<Insulator>();
        }
        protected override WireUtilityNetworkLink AddNetworkLink(GameObject go)
        {
            WireUtilityNetworkLink wireUtilityNetworkLink = base.AddNetworkLink(go);
            wireUtilityNetworkLink.maxWattageRating = WirePatchs.WattageRating.Max1MW.ToWireWattageRating();
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
