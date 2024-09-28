using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace GigaWattWire
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "previews/preview.png")]
    [RestartRequired]
    public class WireOptions
    {
        [Option("Make wire bridge insulated", "If the box is checked, Wire bridges will be insulated.")]
        [JsonProperty]
        public bool MakeWireBridgeInsulated { get; set; }

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

        [Option("Enable 100kW Transformer", "Uncheck this box will remove the 100kW transformer from the mod. It will not remove existing wire in a game.")]
        [JsonProperty]
        public bool Enable100kWTransformer { get; set; }

        [Option("Enable 2MW Transformer", "Uncheck this box will remove the 2MW transformer from the mod. It will not remove existing wire in a game.")]
        [JsonProperty]
        public bool Enable2MWTranformer { get; set; }

        public WireOptions()
        {
            MakeWireBridgeInsulated = false;
            EnableHighWattageWireToPassThroughtWall = false;
            EnableBigWireToPassThroughtWall = false;
            EnableMegaWattWire = true;
            EnableGigaWattWire = true;
            EnableJacketedWire = true;
            Enable100kWTransformer = true;
            Enable2MWTranformer = false;
        }
    }
}

