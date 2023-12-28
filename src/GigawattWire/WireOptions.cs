using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace GigaWattWire
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "previews/preview.png")]
    [RestartRequired]
    public class WireOptions
    {
        [Option("Enable game wire throught wall", "If the box is checked, You will be able to build the vanilla High-wattage and Refined high-wattage wire throught wall.")]
        [JsonProperty]
        public bool EnableHighWattageWireToPassThroughtWall { get; set; }

        [Option("Enable big wire to pass throught wall", "If the box is checked, You will be able to build Mega-watt and Giga-watt wire throught wall.")]
        [JsonProperty]
        public bool EnableBigWireToPassThroughtWall { get; set; }

        [Option("Enable Jacketed Wire", "Uncheck this box will remove jacketed wire from the mod. It will not remove existing wire in a game.")]
        [JsonProperty]
        public bool EnableJacketedWire { get; set; }

        [Option("Enable Mega-Watt Wire", "Uncheck this box will remove mega-watt wire from the mod. It will not remove existing wire in a game.")]
        [JsonProperty]
        public bool EnableMegaWattWire { get; set; }

        [Option("Enable Giga-Watt Wire", "Uncheck this box will remove giga-watt wire from the mod. It will not remove existing wire in a game.")]
        [JsonProperty]
        public bool EnableGigaWattWire { get; set; }

        public WireOptions()
        {
            EnableHighWattageWireToPassThroughtWall = false;
            EnableBigWireToPassThroughtWall = false;
            EnableMegaWattWire = true;
            EnableGigaWattWire = true;
            EnableJacketedWire = true;
        }
    }
}

