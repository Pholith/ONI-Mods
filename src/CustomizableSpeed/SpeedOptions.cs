using Newtonsoft.Json;
using PeterHan.PLib;

namespace CustomizableSpeed
{
    public class SpeedOptions
    {
        [Option("Slow speed", "")]
        [Limit(1, 10)]
        [JsonProperty]
        public int slowSpeed { get; set; }

        [Option("Normal speed", "")]
        [Limit(1, 30)]
        [JsonProperty]
        public int normalSpeed { get; set; }

        [Option("Super speed", "")]
        [Limit(1, 50)]
        [JsonProperty]
        public int superSpeed { get; set; }


        public SpeedOptions()
        {
            slowSpeed = 1;
            normalSpeed = 2;
            superSpeed = 3;
        }
    }
}
