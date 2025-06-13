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
    public class PressureLiquidPumpConfig : IBuildingConfig
    {
        public const string Id = "PressureLiquidPump";

        public const string DisplayName = "High Pressure Liquid Pump";
        public static string Description = string.Concat(new string[] { "An advanced pump that perform mechanical work to compress and move fluids. More powerful than the standard pump, this one is capable of moving large amounts of liquids, although this is only archived through the ", UI.FormatAsLink("High Pressure Liquid Pipe", HighPressureLiquidConduitConfig.Id),"." });
        public static string Effect = string.Concat(new string[]
                {
                    "Draws in ",
                    UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
                    " and runs it through ",
                    UI.FormatAsLink("High Pressure Liquid Pipe", HighPressureLiquidConduitConfig.Id),
                    ".\n\nMust be submerged in ",
                    UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
                    "."
                });

        public override BuildingDef CreateBuildingDef()
        {
            float[] quantity1 = new float[] { 400f, 200f };
            string[] materials1 = new string[] { SimHashes.Steel.ToString(), SimHashes.Polypropylene.ToString() };
            EffectorValues nONE = NOISE_POLLUTION.NONE;
            BuildingDef def1 = BuildingTemplates.CreateBuildingDef("PressureLiquidPump", 2, 3, "pressure_liquid_pump_kanim", 240, 120f, quantity1, materials1, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, nONE, 0.2f);
            def1.RequiresPowerInput = true;
            def1.Overheatable = false;
            def1.EnergyConsumptionWhenActive = ((float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid * 240f) / 10f;
            def1.ExhaustKilowattsWhenActive = 0f;
            def1.SelfHeatKilowattsWhenActive = 2f;
            def1.OutputConduitType = ConduitType.Liquid;
            def1.Floodable = false;
            def1.ViewMode = OverlayModes.LiquidConduits.ID;
            def1.AudioCategory = "Metal";
            def1.PowerInputOffset = new CellOffset(0, 1);
            def1.UtilityOutputOffset = new CellOffset(1, 2);
            def1.PermittedRotations = PermittedRotations.R90;
            def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidPump");
            return def1;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<EnergyConsumer>();
            go.AddOrGet<Pump>();
            go.AddOrGet<Storage>().capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
            ElementConsumer local1 = go.AddOrGet<ElementConsumer>();
            local1.configuration = ElementConsumer.Configuration.AllLiquid;
            local1.consumptionRate = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
            local1.storeOnConsume = true;
            local1.showInStatusPanel = false;
            local1.consumptionRadius = 8;
            ConduitDispenser local2 = go.AddOrGet<ConduitDispenser>();
            local2.conduitType = ConduitType.Liquid;
            local2.alwaysDispense = true;
            local2.elementFilter = null;
            go.AddOrGetDef<OperationalController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
        }
    }
}
