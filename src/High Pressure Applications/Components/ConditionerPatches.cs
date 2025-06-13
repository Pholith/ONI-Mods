using HarmonyLib;
using UnityEngine;
using TUNING;
using PeterHan.PLib.Options;

namespace High_Pressure_Applications.Components
{
    public class ConditionerPatches
    {
        public class Aquatuner_PressurePatch
        {
            [HarmonyPatch(typeof(LiquidConditionerConfig))]
            [HarmonyPatch(nameof(LiquidConditionerConfig.ConfigureBuildingTemplate))]
            public static class Aquatuner_BuildingDef_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                    conduitConsumer.conduitType = ConduitType.Liquid;
                    conduitConsumer.consumptionRate = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
                    Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
                    storage.showInUI = true;
                    storage.capacityKg = 2f * conduitConsumer.consumptionRate;
                }
            }
        }

        public class AirConditioner_PressurePatch
        {
            [HarmonyPatch(typeof(AirConditionerConfig))]
            [HarmonyPatch(nameof(AirConditionerConfig.ConfigureBuildingTemplate))]
            public static class AirConditioner_BuildingDef_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
                    conduitConsumer.conduitType = ConduitType.Gas;
                    conduitConsumer.consumptionRate = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas;
                    Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
                    storage.showInUI = true;
                    storage.capacityKg = 2f * conduitConsumer.consumptionRate;
                }
            }
        }

    }
}
