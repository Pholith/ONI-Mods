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
    public class PressureGasPumpConfig : IBuildingConfig
    {
        public const string Id = "PressureGasPump";

        public const string DisplayName = "High Pressure Gas Pump";
        public static string Description = string.Concat(new string[] { "An advanced pump that perform mechanical work to compress and move gases. More powerful than the standard pump, this one is capable of moving large amounts of gases, although this is only archived through the ", UI.FormatAsLink("High Pressure Gas Pipe", HighPressureGasConduitConfig.Id), "." });
        public static string Effect = string.Concat(new string[]
                {
                    "Draws in ",
                    UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
                    " and runs it through ",
                    UI.FormatAsLink("High Pressure Gas Pipe", HighPressureGasConduitConfig.Id),
                    ".\n\nMust be submerged in ",
                    UI.FormatAsLink("Gas", "ELEMENTS_GAS"),
                    "."
                });

        public override BuildingDef CreateBuildingDef()
        {
            float[] quantity1 = new float[] { 400f, 200f };
            string[] materials1 = new string[] { SimHashes.Steel.ToString(), SimHashes.Polypropylene.ToString() };
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER2;
            BuildingDef def1 = BuildingTemplates.CreateBuildingDef("PressureGasPump", 2, 3, "pressure_gas_pump_kanim", 30, 30f, quantity1, materials1, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, noise, 0.2f);
            def1.RequiresPowerInput = true;
            def1.Overheatable = false;
            def1.EnergyConsumptionWhenActive = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas * 240f;
            def1.ExhaustKilowattsWhenActive = 0f;
            def1.SelfHeatKilowattsWhenActive = 0f;
            def1.OutputConduitType = ConduitType.Gas;
            def1.Floodable = true;
            def1.ViewMode = OverlayModes.GasConduits.ID;
            def1.AudioCategory = "Metal";
            def1.PowerInputOffset = new CellOffset(0, 1);
            def1.UtilityOutputOffset = new CellOffset(0, 2);
            def1.PermittedRotations = PermittedRotations.R90;
            def1.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasPump");
            return def1;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<EnergyConsumer>();
            go.AddOrGet<Pump>();
            go.AddOrGet<Storage>().capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas;
            ElementConsumer local1 = go.AddOrGet<ElementConsumer>();
            local1.configuration = ElementConsumer.Configuration.AllGas;
            local1.consumptionRate = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas;
            local1.storeOnConsume = true;
            local1.showInStatusPanel = false;
            local1.consumptionRadius = 12;
            ConduitDispenser local2 = go.AddOrGet<ConduitDispenser>();
            local2.conduitType = ConduitType.Gas;
            local2.alwaysDispense = true;
            local2.elementFilter = null;
            go.AddOrGetDef<OperationalController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
        }
    }

}
