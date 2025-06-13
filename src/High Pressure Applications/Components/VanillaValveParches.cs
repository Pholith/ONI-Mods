using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace High_Pressure_Applications.Components
{
    //===> GAS LOGIC VALVE PATCH <=============================================================================================
    namespace GasValve_Patches
    {
        public class GasLogicValveConfigPressurePatches
        {
            [HarmonyPatch(typeof(GasLogicValveConfig))]
            [HarmonyPatch(nameof(GasLogicValveConfig.ConfigureBuildingTemplate))]
            public class GasLogicValveConfig_ConfigureBuildingTemplate_Patch
            {
                public static void Postfix(ref GameObject go)
                {
                    OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
                    operationalValve.conduitType = ConduitType.Gas;
                    operationalValve.maxFlow = 10f; //Original game value was 1f
                }
            }

            [HarmonyPatch(typeof(GasLogicValveConfig), "CreateBuildingDef")]
            public static class GasLogicValveConfig_CreateBuildingDef_Patch
            {
                public static void Postfix(BuildingDef __result)
                {
                    __result.Overheatable = false;
                }
            }
        }
    }

    //===> LIQUID LOGIC VALVE PATCH <==========================================================================================
    namespace LiquidValve_Patches
    {
        public class LiquidLogicValveConfigPressurePatches
        {
            [HarmonyPatch(typeof(LiquidLogicValveConfig))]
            [HarmonyPatch(nameof(LiquidLogicValveConfig.ConfigureBuildingTemplate))]
            public class LiquidLogicValveConfig_ConfigureBuildingTemplate_Patch
            {
                public static void Postfix(ref GameObject go)
                {
                    OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
                    operationalValve.conduitType = ConduitType.Liquid;
                    operationalValve.maxFlow = 40f; //Original game value was 10f
                }
            }

            [HarmonyPatch(typeof(LiquidLogicValveConfig), "CreateBuildingDef")]
            public static class LiquidLogicValveConfig_CreateBuildingDef_Patch
            {
                public static void Postfix(BuildingDef __result)
                {
                    __result.Overheatable = false;
                }
            }
        }
    }
}
