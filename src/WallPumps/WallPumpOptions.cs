using Newtonsoft.Json;
using PeterHan.PLib.Options;
using MemberSerialization = Newtonsoft.Json.MemberSerialization;

namespace WallPumps
{
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "preview.png")]
    public class WallPumpOptions
    {
        [Option("STRINGS.UI_ADD.ENABLED", category: "STRINGS.BUILDINGS.PREFABS.FAIRGASWALLPUMP.NAME")]
        [JsonProperty]
        public bool GasWallPumpEnabled { get; set; }
        [Option("STRINGS.UI_ADD.ENERGY_CONSUMPTION", category: "STRINGS.BUILDINGS.PREFABS.FAIRGASWALLPUMP.NAME")]
        [JsonProperty]
        public float GasWallPumpEnergyConsumption { get; set; }
        [Option("STRINGS.UI_ADD.PUMP_RATE", category: "STRINGS.BUILDINGS.PREFABS.FAIRGASWALLPUMP.NAME")]
        [JsonProperty]
        public float GasWallPumpRate{ get; set; }
        [Option("STRINGS.UI_ADD.THERMAL_CONDUCTIVITY", category: "STRINGS.BUILDINGS.PREFABS.FAIRGASWALLPUMP.NAME")]
        [JsonProperty]
        public float GasWallPumpThermalConductivity{ get; set; }

        [Option("STRINGS.UI_ADD.ENABLED", category: "STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLPUMP.NAME")]
        [JsonProperty]
        public bool LiquidWallPumpEnabled { get; set; }
        [Option("STRINGS.UI_ADD.ENERGY_CONSUMPTION", category: "STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLPUMP.NAME")]
        [JsonProperty]
        public float LiquidWallPumEnergyConsumption { get; set; }
        [Option("STRINGS.UI_ADD.PUMP_RATE", category: "STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLPUMP.NAME")]
        [JsonProperty]
        public float LiquidWallPumpRate { get; set; }
        [Option("STRINGS.UI_ADD.THERMAL_CONDUCTIVITY", category: "STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLPUMP.NAME")]
        [JsonProperty]
        public float LiquidWallPumpThermalConductivity { get; set; }

        [Option("STRINGS.UI_ADD.ENABLED", category: "STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENT.NAME")]
        [JsonProperty]
        public bool GasWallVentEnabled { get; set; }
        [Option("STRINGS.UI_ADD.MAX_PRESSURE", category: "STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENT.NAME")]
        [JsonProperty]
        public float GasWallVentMaxPressure{ get; set; }
        [Option("STRINGS.UI_ADD.THERMAL_CONDUCTIVITY", category: "STRINGS.BUILDINGS.PREFABS.FAIRGASWALLVENT.NAME")]
        [JsonProperty]
        public float GasWallVentThermalConductivity { get; set; }

        [Option("STRINGS.UI_ADD.ENABLED", category: "STRINGS.BUILDINGS.PREFABS.VENTHIGHPRESSURE.NAME")]
        [JsonProperty]
        public bool GasWallPressureVentEnabled { get; set; }
        [Option("STRINGS.UI_ADD.MAX_PRESSURE", category: "STRINGS.BUILDINGS.PREFABS.VENTHIGHPRESSURE.NAME")]
        [JsonProperty]
        public float GasWallPressureMaxPressure { get; set; }
        [Option("STRINGS.UI_ADD.THERMAL_CONDUCTIVITY", category: "STRINGS.BUILDINGS.PREFABS.VENTHIGHPRESSURE.NAME")]
        [JsonProperty]
        public float GasWallPressureThermalConductivity { get; set; }

        [Option("STRINGS.UI_ADD.ENABLED", category: "STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLVENT.NAME")]
        [JsonProperty]
        public bool LiquidWallVentEnabled { get; set; }
        [Option("STRINGS.UI_ADD.MAX_PRESSURE", category: "STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLVENT.NAME")]
        [JsonProperty]
        public float LiquidWallVentMaxPressure{ get; set; }
        [Option("STRINGS.UI_ADD.THERMAL_CONDUCTIVITY", category: "STRINGS.BUILDINGS.PREFABS.FAIRLIQUIDWALLVENT.NAME")]
        [JsonProperty]
        public float LiquidWallVentThermalConductivity { get; set; }


        public WallPumpOptions()
        {
            GasWallPumpEnabled = true;
            GasWallPumpEnergyConsumption = 120;
            GasWallPumpRate = 0.2f;
            GasWallPumpThermalConductivity = 1;

            LiquidWallPumpEnabled = true;
            LiquidWallPumEnergyConsumption = 120;
            LiquidWallPumpRate = 4;
            LiquidWallPumpThermalConductivity = 1;

            GasWallVentEnabled = true;
            GasWallVentMaxPressure = 2;
            GasWallVentThermalConductivity = 1;

            GasWallPressureVentEnabled = true;
            GasWallPressureMaxPressure = 20;
            GasWallPressureThermalConductivity = 1;

            LiquidWallVentEnabled = true;
            LiquidWallVentMaxPressure = 1000;
            LiquidWallVentThermalConductivity = 1;
        }
    }
}

