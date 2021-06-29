using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace CustomizableSpeed
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Pholith/ONI-Mods")]
    public class SpeedOptions
    {
        [Option("Slow speed", "")]
        [Limit(0, 10)]
        [JsonProperty]
        public float slowSpeed { get; set; }

        [Option("Normal speed", "")]
        [Limit(0, 20)]
        [JsonProperty]
        public float normalSpeed { get; set; }

        [Option("Super speed", "")]
        [Limit(0, 30)]
        [JsonProperty]
        public float superSpeed { get; set; }


        public SpeedOptions()
        {
            slowSpeed = 1;
            normalSpeed = 2;
            superSpeed = 3;
        }
    }
}
