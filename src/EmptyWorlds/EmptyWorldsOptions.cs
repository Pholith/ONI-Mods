using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;

namespace EmptyWorlds
{
    [ModInfo("EmptyWorlds - Skyblock", "https://github.com/Pholith/ONI-Mods", "screen1.png")]
    [RestartRequired]
    public class EmptyWorldsOptions
    {
        [Option("Remove bunker at worldgen", "Remove the bunker when generating a game on Emptera or Islands")]
        [JsonProperty]
        public bool RemoveBunkerAtWorldgen { get; set; }

        public EmptyWorldsOptions()
        {
            RemoveBunkerAtWorldgen = false;
        }
    }
}

