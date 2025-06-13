using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using PeterHan.PLib.Options;
using High_Pressure_Applications.Components;
using BUILDINGS = TUNING.BUILDINGS;

namespace High_Pressure_Applications.BuildingConfigs
{
    public class HighPressureGasConduitConfig : IBuildingConfig
    {
        public const string Id = "HighPressureGasConduit";
        public const string DisplayName = "High Pressure Gas Conduit";
        public const string Description = "A reinforced gas pipe capable of handling high pressure flow. Composite nature of the pipe prevents gas contents from significantly changing temperature in transit.";
        public static string Effect = string.Concat(new string[]
                {
                    "Carries a maximum of " +
                    (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas,
                    "kg of ",
                    UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
                    " with minimal change in ",
                    UI.FormatAsLink("Temperature", "HEAT"),
                    ".\n\nCan be run through wall and floor tile."
                });

        public override BuildingDef CreateBuildingDef()
        {
            string id = Id;
            int width = 1;
            int height = 1;
            string anim = "pressure_gas_pipe_kanim";
            int hitpoints = 10;
            float construction_time = 30f;
            float[] tIER = { 10f, 5f };
            string[] constructionMaterial = { SimHashes.Steel.ToString(), MATERIALS.PLASTIC };
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, constructionMaterial, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER0, nONE);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = false;
            buildingDef.ThermalConductivity = 1.0e-05f;
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            buildingDef.ObjectLayer = ObjectLayer.GasConduit;
            buildingDef.TileLayer = ObjectLayer.GasConduitTile;
            buildingDef.ReplacementLayer = ObjectLayer.ReplacementGasConduit;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = 0f;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.SceneLayer = Grid.SceneLayer.GasConduits;
            buildingDef.isKAnimTile = true;
            buildingDef.isUtility = true;
            buildingDef.DragBuild = true;
            buildingDef.ReplacementTags = new List<Tag>();
            buildingDef.ReplacementTags.Add(GameTags.Vents);
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, buildingDef.PrefabID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            Conduit conduit = go.AddOrGet<Conduit>();
            conduit.type = ConduitType.Gas;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
            go.AddComponent<EmptyConduitWorkable>();
            KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
            kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Gas;
            kAnimGraphTileVisualizer.isPhysicalBuilding = true;
            go.GetComponent<KPrefabID>().AddTag(GameTags.Vents);
            LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
            kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Gas;
            kAnimGraphTileVisualizer.isPhysicalBuilding = false;
        }
    }
}
