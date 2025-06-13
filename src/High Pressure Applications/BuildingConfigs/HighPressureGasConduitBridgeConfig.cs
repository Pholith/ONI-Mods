using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace High_Pressure_Applications.BuildingConfigs
{
    public class HighPressureGasConduitBridgeConfig : IBuildingConfig
    {
        public const string Id = "HighPressureGasConduitBridge";
        public const string DisplayName = "High Pressure Gas Conduit Bridge";
        public const string Description = "A reinforced gas pipe bridge capable of handling high pressure flow. Composite nature of the pipe prevents gas contents from significantly changing temperature in transit.";
        public static string Effect = "Runs one High Pressure Gas Pipe section over another without joining them.\n\nCan be run through wall and floor tile.";


        private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

        public override BuildingDef CreateBuildingDef()
        {
            string id = Id;
            int width = 3;
            int height = 1;
            string anim = "pressure_gas_bridge_kanim";
            int hitpoints = 10;
            float construction_time = 45f;
            float[] tIER = { 10f, 5f };
            string[] constructionMaterial = { SimHashes.Steel.ToString(), MATERIALS.PLASTIC };
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Conduit;
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, constructionMaterial, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, nONE);
            buildingDef.ObjectLayer = ObjectLayer.GasConduitConnection;
            buildingDef.SceneLayer = Grid.SceneLayer.GasConduitBridges;
            buildingDef.InputConduitType = ConduitType.Gas;
            buildingDef.OutputConduitType = ConduitType.Gas;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.ThermalConductivity = 1.0e-05f;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, buildingDef.PrefabID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            ConduitBridge conduitBridge = go.AddOrGet<ConduitBridge>();
            conduitBridge.type = ConduitType.Gas;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
            UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
        }
    }
}
