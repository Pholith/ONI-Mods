using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace DiscordRPMod
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen1.png")]
    public class DiscordRPOptions
    {
        [Option("Enabled", "If the box is unchecked, disable the discord presence.")]
        [JsonProperty]
        public bool ModEnabled { get; set; }

        [Option("Show save name", "If the box is checked, rich presence will show the game name.")]
        [JsonProperty]
        public bool ShowGameName { get; set; }

        [Option("Show cycle and dups", "If the box is checked, rich presence will show the cycle and the number of dups.")]
        [JsonProperty]
        public bool ShowCycleAndDups{ get; set; }

        [Option("Show asteroid", "If the box is checked, rich presence will show the worldgen asteroid.")]
        [JsonProperty]
        public bool ShowAsteroid{ get; set; }

        public DiscordRPOptions()
        {
            ModEnabled = true;
            ShowGameName = true;
            ShowCycleAndDups = true;
            ShowAsteroid = true;
        }
    }
}

