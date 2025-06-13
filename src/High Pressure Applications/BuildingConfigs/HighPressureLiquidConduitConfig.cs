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
    public class HighPressureLiquidConduitConfig : IBuildingConfig
    {
        public const string Id = "HighPressureLiquidConduit";
        public const string DisplayName = "High Pressure Liquid Pipe";
        public const string Description = "A reinforced liquid pipe capable of handling high pressure flow. Composite nature of the pipe prevents liquid contents from significantly changing temperature in transit.";
        public static string Effect = string.Concat(new string[]
                {
                    "Carries a maximum of " +
                    (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid,
                    "kg of ",
                    UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
                    " with minimal change in ",
                    UI.FormatAsLink("Temperature", "HEAT"),
                    ".\n\nCan be run through wall and floor tile."
                });

        public override BuildingDef CreateBuildingDef()
        {
            string id = Id;
            int width = 1;
            int height = 1;
            string anim = "pressure_liquid_pipe_kanim";
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
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.ThermalConductivity = 1.3f;
            buildingDef.ObjectLayer = ObjectLayer.LiquidConduit;
            buildingDef.TileLayer = ObjectLayer.LiquidConduitTile;
            buildingDef.ReplacementLayer = ObjectLayer.ReplacementLiquidConduit;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduits;
            buildingDef.isKAnimTile = true;
            buildingDef.isUtility = true;
            buildingDef.DragBuild = true;
            buildingDef.ReplacementTags = new List<Tag>();
            buildingDef.ReplacementTags.Add(GameTags.Pipes);
            buildingDef.ThermalConductivity = 1.0e-05f;
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, Id);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            Conduit conduit = go.AddOrGet<Conduit>();
            conduit.type = ConduitType.Liquid;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
            go.AddComponent<EmptyConduitWorkable>();
            KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
            kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
            kAnimGraphTileVisualizer.isPhysicalBuilding = true;
            go.GetComponent<KPrefabID>().AddTag(GameTags.Pipes);
            LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            KAnimGraphTileVisualizer kAnimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
            kAnimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
            kAnimGraphTileVisualizer.isPhysicalBuilding = false;
        }
    }
}
