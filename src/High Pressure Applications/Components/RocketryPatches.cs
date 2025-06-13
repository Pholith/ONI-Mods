using HarmonyLib;
using UnityEngine;
using TUNING;
using PeterHan.PLib.Options;

namespace High_Pressure_Applications.Components
{
    //===> MODULAR GAS PORTS <==============================================================================================
    namespace ModularGasPorts_Patches
    {
        public class ModularGasPort_Loader
        {
            [HarmonyPatch(typeof(ModularLaunchpadPortGasConfig))]
            [HarmonyPatch(nameof(ModularLaunchpadPortGasConfig.ConfigureBuildingTemplate))]
            public static class GasPortLoader_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas, (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas, true);
                }
            }
        }

        public class ModularGasPort_Unloader
        {
            [HarmonyPatch(typeof(ModularLaunchpadPortGasUnloaderConfig))]
            [HarmonyPatch(nameof(ModularLaunchpadPortGasUnloaderConfig.ConfigureBuildingTemplate))]
            public static class GasPortUnloader_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Gas, (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas, false);
                }
            }
        }
    }

    //===> MODULAR LIQUID PORTS <===========================================================================================
    namespace ModularLiquidPort_Patches
    {
        public class ModularLiquidPort_Loader
        {
            [HarmonyPatch(typeof(ModularLaunchpadPortLiquidConfig))]
            [HarmonyPatch(nameof(ModularLaunchpadPortLiquidConfig.ConfigureBuildingTemplate))]
            public static class LiquidPortLoader_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Liquid, (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid, true);
                }
            }
        }

        public class ModularLiquidPort_Unloader
        {
            [HarmonyPatch(typeof(ModularLaunchpadPortLiquidUnloaderConfig))]
            [HarmonyPatch(nameof(ModularLaunchpadPortLiquidUnloaderConfig.ConfigureBuildingTemplate))]
            public static class LiquidPortUnloader_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    BaseModularLaunchpadPortConfig.ConfigureBuildingTemplate(go, prefab_tag, ConduitType.Liquid, (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid, false);
                }
            }
        }
    }

    //===> INTERIOR GAS PORTS <=============================================================================================
    namespace InteiorGasPorts_Patches
    {
        public class InteriorGas_Input
        {
            [HarmonyPatch(typeof(RocketInteriorGasInputConfig))]
            [HarmonyPatch(nameof(RocketInteriorGasInputConfig.DoPostConfigureComplete))]
            public static class InteriorGas_Input_Mod
            {
                public static void Postfix(GameObject go)
                {
                    Storage storage = go.AddOrGet<Storage>();
                    storage.capacityKg = 10f;
                }
            }
        }

        public class InteriorGasPort_Input
        {
            [HarmonyPatch(typeof(RocketInteriorGasInputPortConfig))]
            [HarmonyPatch(nameof(RocketInteriorGasInputPortConfig.ConfigureBuildingTemplate))]
            public static class InteriorGasPort_Input_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    Storage storage = go.AddComponent<Storage>();
                    storage.showInUI = false;
                    storage.capacityKg = 10f;
                }
            }
        }

        public class InteriorGas_Output
        {
            [HarmonyPatch(typeof(RocketInteriorGasOutputConfig))]
            [HarmonyPatch(nameof(RocketInteriorGasOutputConfig.DoPostConfigureComplete))]
            public static class InteriorGas_Output_Mod
            {
                public static void Postfix(GameObject go)
                {
                    Storage storage = go.AddOrGet<Storage>();
                    storage.capacityKg = 10f;
                    go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Gas;
                    RocketConduitStorageAccess rocketConduitStorageAccess = go.AddOrGet<RocketConduitStorageAccess>();
                    rocketConduitStorageAccess.storage = storage;
                    rocketConduitStorageAccess.cargoType = CargoBay.CargoType.Gasses;
                    rocketConduitStorageAccess.targetLevel = 1f;
                }
            }
        }
    }

    //===> INTERIOR LIQUID PORTS <==========================================================================================
    namespace InteriorLiquidPorts_Patches
    {
        public class InteriorLiquid_Input
        {
            [HarmonyPatch(typeof(RocketInteriorLiquidInputConfig))]
            [HarmonyPatch(nameof(RocketInteriorLiquidInputConfig.DoPostConfigureComplete))]
            public static class InteriorLiquid_Input_Mod
            {
                public static void Postfix(GameObject go)
                {
                    Storage storage = go.AddOrGet<Storage>();
                    storage.capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
                }
            }
        }

        public class InteriorLiquidPort_Input
        {
            [HarmonyPatch(typeof(RocketInteriorLiquidInputPortConfig))]
            [HarmonyPatch(nameof(RocketInteriorLiquidInputPortConfig.ConfigureBuildingTemplate))]
            public static class InteriorLiquidPort_Input_Mod
            {
                public static void Postfix(GameObject go, Tag prefab_tag)
                {
                    Storage storage = go.AddComponent<Storage>();
                    storage.showInUI = false;
                    storage.capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
                }
            }
        }

        public class InteriorLiquid_Output
        {
            [HarmonyPatch(typeof(RocketInteriorLiquidOutputConfig))]
            [HarmonyPatch(nameof(RocketInteriorLiquidOutputConfig.DoPostConfigureComplete))]
            public static class InteriorLiquid_Output_Mod
            {
                public static void Postfix(GameObject go)
                {
                    Storage storage = go.AddOrGet<Storage>();
                    storage.capacityKg = 50f;
                    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
                    go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Liquid;
                    RocketConduitStorageAccess rocketConduitStorageAccess = go.AddOrGet<RocketConduitStorageAccess>();
                    rocketConduitStorageAccess.storage = storage;
                    rocketConduitStorageAccess.cargoType = CargoBay.CargoType.Liquids;
                    rocketConduitStorageAccess.targetLevel = 50f;
                }
            }
        }
    }

    //===> TELEPORTER <=====================================================================================================
    namespace Teleporter_Patches
    {
        public class WarpConduitSender_Capacity
        {
            [HarmonyPatch(typeof(WarpConduitSenderConfig))]
            [HarmonyPatch(nameof(WarpConduitSenderConfig.ConfigureBuildingTemplate))]
            public static class WarpConduitSender_Capacity_Mod
            {
                public static void Prefix(GameObject go, Tag prefab_tag)
                {
                    WarpConduitSender warpConduitSender = go.AddOrGet<WarpConduitSender>();
                    warpConduitSender.gasStorage = go.AddComponent<Storage>();
                    warpConduitSender.gasStorage.showInUI = false;
                    warpConduitSender.gasStorage.capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas;
                    warpConduitSender.liquidStorage = go.AddComponent<Storage>();
                    warpConduitSender.liquidStorage.showInUI = false;
                    warpConduitSender.liquidStorage.capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
                    warpConduitSender.solidStorage = go.AddComponent<Storage>();
                    warpConduitSender.solidStorage.showInUI = false;
                    warpConduitSender.solidStorage.capacityKg = 1000f;
                }
            }
        }

        public class WarpConduitReceiver_Capacity
        {
            [HarmonyPatch(typeof(WarpConduitReceiverConfig))]
            [HarmonyPatch(nameof(WarpConduitReceiverConfig.ConfigureBuildingTemplate))]
            public static class WarpConduitReceiver_Capacity_Mod
            {
                public static void Prefix(GameObject go, Tag prefab_tag)
                {
                    WarpConduitReceiver warpConduitReceiver = go.AddOrGet<WarpConduitReceiver>();
                    warpConduitReceiver.senderGasStorage = go.AddComponent<Storage>();
                    warpConduitReceiver.senderGasStorage.showInUI = false;
                    warpConduitReceiver.senderGasStorage.capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas;
                    warpConduitReceiver.senderLiquidStorage = go.AddComponent<Storage>();
                    warpConduitReceiver.senderLiquidStorage.showInUI = false;
                    warpConduitReceiver.senderLiquidStorage.capacityKg = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid;
                    warpConduitReceiver.senderSolidStorage = go.AddComponent<Storage>();
                    warpConduitReceiver.senderSolidStorage.showInUI = false;
                    warpConduitReceiver.senderSolidStorage.capacityKg = 1000f;

                }
            }
        }
    }

}
