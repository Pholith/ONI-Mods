using HarmonyLib;
using System;
using UnityEngine;

namespace High_Pressure_Applications.BuildingConfigs
{

    //===> PRESSURE GAS PUMP <=================================================================================================
    namespace PressureGasPump_Patches
    {
        namespace PressureGasPump_TechPatch
        {
            [HarmonyPatch(typeof(Db), "Initialize")]
            internal class PressureGasPumpTechMod
            {
                private static void Postfix()
                {
                    Db.Get().Techs.Get("ValveMiniaturization").unlockedItemIDs.Add(PressureGasPumpConfig.Id);
                }
            }
        }

        namespace PressureGasPump_UIPatch
        {
            [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
            internal class PressureGasPumpUI
            {
                private static void Prefix()
                {
                    string[] textArray1 = new string[] { "STRINGS.BUILDINGS.PREFABS.PRESSUREGASPUMP.NAME", PressureGasPumpConfig.DisplayName };
                    Strings.Add(textArray1);
                    string[] textArray2 = new string[] { "STRINGS.BUILDINGS.PREFABS.PRESSUREGASPUMP.DESC", PressureGasPumpConfig.Description };
                    Strings.Add(textArray2);
                    string[] textArray3 = new string[] { "STRINGS.BUILDINGS.PREFABS.PRESSUREGASPUMP.EFFECT", PressureGasPumpConfig.Effect };
                    Strings.Add(textArray3);
                    ModUtil.AddBuildingToPlanScreen("HVAC", PressureGasPumpConfig.Id);
                }
            }
        }
    }

    //===> PRESSURE LIQUID PUMP <==============================================================================================
    namespace PressureLiquidPump_Patches
    {
        namespace PressureLiquidPump_TechPatch
        {
            [HarmonyPatch(typeof(Db), "Initialize")]
            internal class PressureLiquidPumpTechMod
            {
                private static void Postfix()
                {
                    Db.Get().Techs.Get("ValveMiniaturization").unlockedItemIDs.Add(PressureLiquidPumpConfig.Id);
                }
            }
        }

        namespace PressureLiquidPump_UIPatch
        {
            [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
            internal class PressureLiquidPumpUI
            {
                private static void Prefix()
                {
                    string[] textArray1 = new string[] { "STRINGS.BUILDINGS.PREFABS.PRESSURELIQUIDPUMP.NAME", PressureLiquidPumpConfig.DisplayName };
                    Strings.Add(textArray1);
                    string[] textArray2 = new string[] { "STRINGS.BUILDINGS.PREFABS.PRESSURELIQUIDPUMP.DESC", PressureLiquidPumpConfig.Description };
                    Strings.Add(textArray2);
                    string[] textArray3 = new string[] { "STRINGS.BUILDINGS.PREFABS.PRESSURELIQUIDPUMP.EFFECT", PressureLiquidPumpConfig.Effect };
                    Strings.Add(textArray3);
                    ModUtil.AddBuildingToPlanScreen("Plumbing", PressureLiquidPumpConfig.Id);
                }
            }
        }
    }

    //===> DECOMPRESSION GAS VALVE <===========================================================================================
    namespace DecompressionGasValve_Patches
    {
        namespace DecompressionGasValve_TechPatch
        {
            [HarmonyPatch(typeof(Db), "Initialize")]
            internal class DecompressionGasValveTechMod
            {
                private static void Postfix()
                {
                    Db.Get().Techs.Get("HVAC").unlockedItemIDs.Add(DecompressionGasValveConfig.Id);
                }
            }
        }

        namespace DecompressionGasValve_UIPatch
        {
            [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
            internal class DecompressionGasValveUI
            {
                private static void Prefix()
                {
                    string[] textArray1 = new string[] { "STRINGS.BUILDINGS.PREFABS.DECOMPRESSIONGASVALVE.NAME", DecompressionGasValveConfig.DisplayName };
                    Strings.Add(textArray1);
                    string[] textArray2 = new string[] { "STRINGS.BUILDINGS.PREFABS.DECOMPRESSIONGASVALVE.DESC", DecompressionGasValveConfig.Description };
                    Strings.Add(textArray2);
                    string[] textArray3 = new string[] { "STRINGS.BUILDINGS.PREFABS.DECOMPRESSIONGASVALVE.EFFECT", DecompressionGasValveConfig.Effect };
                    Strings.Add(textArray3);
                    ModUtil.AddBuildingToPlanScreen("HVAC", DecompressionGasValveConfig.Id);
                }
            }
        }
    }

    //===> DECOMPRESSION LIQUID VALVE <=========================================================================================
    namespace DecompressionLiquidValve_Patches
    {
        namespace DecompressionLiquidValve_TechPatch
        {
            [HarmonyPatch(typeof(Db), "Initialize")]
            internal class DecompressionLiquidValveTechMod
            {
                private static void Postfix()
                {
                    Db.Get().Techs.Get("LiquidTemperature").unlockedItemIDs.Add(DecompressionLiquidValveConfig.Id);
                }
            }
        }

        namespace DecompressionLiquidValve_UIPatch
        {
            [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
            internal class DecompressionLiquidValveUI
            {
                private static void Prefix()
                {
                    string[] textArray1 = new string[] { "STRINGS.BUILDINGS.PREFABS.DECOMPRESSIONLIQUIDVALVE.NAME", DecompressionLiquidValveConfig.DisplayName };
                    Strings.Add(textArray1);
                    string[] textArray2 = new string[] { "STRINGS.BUILDINGS.PREFABS.DECOMPRESSIONLIQUIDVALVE.DESC", DecompressionLiquidValveConfig.Description };
                    Strings.Add(textArray2);
                    string[] textArray3 = new string[] { "STRINGS.BUILDINGS.PREFABS.DECOMPRESSIONLIQUIDVALVE.EFFECT", DecompressionLiquidValveConfig.Effect };
                    Strings.Add(textArray3);
                    ModUtil.AddBuildingToPlanScreen("Plumbing", DecompressionLiquidValveConfig.Id);
                }
            }
        }
    }
}
