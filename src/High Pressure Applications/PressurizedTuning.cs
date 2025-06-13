using System.Collections.Generic;
using UnityEngine;
using High_Pressure_Applications.BuildingConfigs;
using PeterHan.PLib.Options;
using High_Pressure_Applications.Components;

namespace High_Pressure_Applications
{
    public class PressurizedInfo
    {   
        public float Capacity;
        public float IncreaseMultiplier;
        public Color32 OverlayTint; //The tint applied to the overlay appearance of the sprite
        public Color32 FlowTint; //The tint applied to the flowing sprites within the conduits
        public Color32 FlowOverlayTint; //The tint tint applied to the overlay appearance of the flowing sprites
        public bool IsDefault;
        public bool CanInsulate = false;
        public float InsulateCost = -1f;
    }

    public static class PressurizedTuning
    {
        public static PressurizedInfo GetPressurizedInfo(string id)
        {
            if (PressurizedLookup.ContainsKey(id))
                return PressurizedLookup[id];
            else
                return PressurizedLookup[""];
        }

        public static bool TryAddPressurizedInfo(string id, PressurizedInfo info)
        {
            if (PressurizedLookup.ContainsKey(id))
            {
                Debug.LogWarning($"[Pressurized] PressurizedTuning.TryAddPressurizedInfo(string, PressurizedInfo) -> Attempted to add an id that already exists.");
                return false;
            }
            else if (info == null || info.IsDefault == true || info.Capacity <= 0f)
            {
                Debug.LogWarning($"[Pressurized] PressurizedTuning.TryAddPressurizedInfo(string, PressurizedInfo) -> PressurizedInfo argument was invalid. Must not be null, have a Capacity > 0, and IsDefault must be false.");
                return false;
            }
            PressurizedLookup.Add(id, info);
            return true;
        }

        private static Dictionary<string, PressurizedInfo> PressurizedLookup = new Dictionary<string, PressurizedInfo>()
        {
            {
                HighPressureGasConduitConfig.Id,
                new PressurizedInfo()
                {
                    Capacity = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas,
                    IncreaseMultiplier = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas,
                    FlowTint = new Color32(176, 176, 176, 255),
                    FlowOverlayTint = new Color32(176, 176, 176, 255),
                    IsDefault = false,
                    CanInsulate = true,
                    InsulateCost = 400f
                }
            },
            {

                HighPressureLiquidConduitConfig.Id,
                new PressurizedInfo()
                {
                    Capacity = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid,
                    IncreaseMultiplier = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid,
                    FlowTint = new Color32(92, 144, 121, 25),
                    FlowOverlayTint = new Color32(92, 144, 121, 255),
                    IsDefault = false,
                    CanInsulate = true,
                    InsulateCost = 400f
                }
            },
            {
                HighPressureGasConduitBridgeConfig.Id,
                new PressurizedInfo()
                {
                    Capacity = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas,
                    IncreaseMultiplier = (float)SingletonOptions<HPA_ModSettings>.Instance.HPGas,
                    IsDefault = false,
                    CanInsulate = true,
                    InsulateCost = 400f
                }
            },
            {
                HighPressureLiquidConduitBridgeConfig.Id,
                new PressurizedInfo()
                {
                    Capacity = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid,
                    IncreaseMultiplier = (float)SingletonOptions<HPA_ModSettings>.Instance.HPLiquid,
                    IsDefault = false,
                    CanInsulate = true,
                    InsulateCost = 400f
                }
            },
            {
                "",
                new PressurizedInfo()
                {
                    Capacity = -1f,
                    IncreaseMultiplier = 3f,
                    FlowTint = new Color32(255, 255, 255, 255),
                    IsDefault = true,
                    CanInsulate = false
                }
            }
        };
    }
}
