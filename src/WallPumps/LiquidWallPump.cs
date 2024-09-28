using UnityEngine;
using TUNING;
using FairONI;
using STRINGS;

namespace WallPumps
{
    public class LiquidWallPump : IBuildingConfig
    {
        public const string ID = "FairLiquidWallPump";
       

        public static void AddToMenus()
        {
            if (GameOnLoadPatch.Settings.LiquidWallPumpEnabled)
            {
                AddBuilding.AddBuildingToPlanScreen("Plumbing", ID, "LiquidPump");
                AddBuilding.IntoTechTree("ImprovedLiquidPiping", ID);
            }
        }

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                ID,
                1,
                1,
                "fairliquidwallpump_kanim",
                30,
                30f,
                TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
                MATERIALS.REFINED_METALS,
                1600f,
                BuildLocationRule.Tile,
                TUNING.BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NOISY.TIER2,
                0.2f);
            BuildingTemplates.CreateFoundationTileDef(def);

            def.RequiresPowerInput = true;
            def.EnergyConsumptionWhenActive = GameOnLoadPatch.Settings.LiquidWallPumEnergyConsumption;
            def.ExhaustKilowattsWhenActive = 0f;
            def.SelfHeatKilowattsWhenActive = 0f;
            def.OutputConduitType = ConduitType.Liquid;
            def.Floodable = false;
            def.ViewMode = OverlayModes.LiquidConduits.ID;
            def.AudioCategory = "Metal";
            def.PowerInputOffset = new CellOffset(0, 0);
            def.UtilityOutputOffset = new CellOffset(0, 0);
            def.PermittedRotations = PermittedRotations.R360;
            // Tile properties
            def.ThermalConductivity = GameOnLoadPatch.Settings.LiquidWallPumpThermalConductivity;
            def.UseStructureTemperature = false;
            def.Entombable = false;
            def.BaseTimeUntilRepair = -1f;
            def.ObjectLayer = ObjectLayer.Building;
            def.SceneLayer = Grid.SceneLayer.TileMain;
            def.ForegroundLayer = Grid.SceneLayer.TileMain;

            // Insulated option
            if (GameOnLoadPatch.Settings.AreTilesInsulated) def.ThermalConductivity = 0.01f;

            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
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
            //go.AddOrGetDef<StorageController.Def>();

            GeneratedBuildings.RegisterSingleLogicInputPort(go);
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<EnergyConsumer>();
            go.AddOrGet<RotatablePump>();
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = GameOnLoadPatch.Settings.LiquidWallPumpRate * 2;
            RotatableElementConsumer elementConsumer = go.AddOrGet<RotatableElementConsumer>();
            elementConsumer.configuration = ElementConsumer.Configuration.AllLiquid;
            elementConsumer.consumptionRate = GameOnLoadPatch.Settings.LiquidWallPumpRate;
            elementConsumer.storeOnConsume = true;
            elementConsumer.showInStatusPanel = false;
            elementConsumer.rotatableCellOffset = new Vector3(0, 1);
            elementConsumer.consumptionRadius = 2;
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.elementFilter = null;
            go.AddOrGetDef<OperationalController.Def>();
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
