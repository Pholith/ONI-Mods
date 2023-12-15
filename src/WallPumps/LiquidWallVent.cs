using UnityEngine;
using TUNING;
using FairONI;
using STRINGS;

namespace WallPumps
{
    public class LiquidWallVent : IBuildingConfig
    {
        public const string ID = "FairLiquidWallVent";
        
        public static void AddToMenus()
        {
            if (GameOnLoadPatch.Settings.LiquidWallVentEnabled)
            {
                AddBuilding.AddBuildingToPlanScreen("Plumbing", ID, "LiquidVent");
                AddBuilding.IntoTechTree("LiquidPiping", ID);
            }
        }

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                ID,
                1,
                1,
                "fairliquidwallvent_kanim",
                30,
                30f,
                TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
                MATERIALS.RAW_METALS,
                1600f,
                BuildLocationRule.Tile,
                TUNING.BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NONE,
                0.2f);
            BuildingTemplates.CreateFoundationTileDef(def);
            
            def.InputConduitType = ConduitType.Liquid;
            def.Floodable = false;
            def.Overheatable = false;
            def.ViewMode = OverlayModes.LiquidConduits.ID;
            def.AudioCategory = "Metal";
            def.UtilityInputOffset = new CellOffset(0, 0);
            def.UtilityOutputOffset = new CellOffset(0, 1);
            def.PermittedRotations = PermittedRotations.R360;
            // Tile properties
            def.ThermalConductivity = GameOnLoadPatch.Settings.LiquidWallVentThermalConductivity;
            def.UseStructureTemperature = false;
            def.Entombable = false;
            def.BaseTimeUntilRepair = -1f;
            def.ObjectLayer = ObjectLayer.Building;
            def.SceneLayer = Grid.SceneLayer.TileMain;
            def.ForegroundLayer = Grid.SceneLayer.TileMain;
            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.RegisterSingleLogicInputPort(go);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<RotatableExhaust>();
            Vent vent = go.AddOrGet<Vent>();
            vent.conduitType = ConduitType.Liquid;
            vent.endpointType = Endpoint.Sink;
            vent.overpressureMass = GameOnLoadPatch.Settings.LiquidWallVentMaxPressure;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
            storage.showInUI = true;
            go.AddOrGet<SimpleVent>();
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<Insulator>();
            go.AddOrGet<TileTemperature>();
            BuildingHP buildingHP = go.AddOrGet<BuildingHP>();
            buildingHP.destroyOnDamaged = true;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            AddVisualizer(go, true);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            AddVisualizer(go, false);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<VentController.Def>();
            AddVisualizer(go, false);

            GeneratedBuildings.RemoveLoopingSounds(go);
        }

        private static void AddVisualizer(GameObject go, bool movable)
        {
            RangeVisualizer _RangeVisualizer = go.AddOrGet<RangeVisualizer>();

            _RangeVisualizer.OriginOffset = new Vector2I(0, 1);
            _RangeVisualizer.RangeMin.x = 0;
            _RangeVisualizer.RangeMin.y = 0;
            _RangeVisualizer.RangeMax.x = 0;
            _RangeVisualizer.RangeMax.y = 0;
        }
    }
}
