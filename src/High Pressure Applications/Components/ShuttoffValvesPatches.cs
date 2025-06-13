using HarmonyLib;
using UnityEngine;
using TUNING;
using PeterHan.PLib.Options;

namespace High_Pressure_Applications.Components
{
    public class ShuttoffValvesPatches
    {
        public class GasLogicValve_PressurePatch
        {
            [HarmonyPatch(typeof(GasLogicValveConfig))]
            [HarmonyPatch(nameof(GasLogicValveConfig.ConfigureBuildingTemplate))]
            public static class GasLogicValve_BuildingDef_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
                    operationalValve.conduitType = ConduitType.Gas;
                    operationalValve.maxFlow = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas;

                }
            }
        }

        public class LiquidLogicValve_PressurePatch
        {
            [HarmonyPatch(typeof(LiquidLogicValveConfig))]
            [HarmonyPatch(nameof(LiquidLogicValveConfig.ConfigureBuildingTemplate))]
            public static class LiquidLogicValve_BuildingDef_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
                    operationalValve.conduitType = ConduitType.Liquid;
                    operationalValve.maxFlow = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
                }
            }
        }
    }
}
