using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace HeliumExtractor
{
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen.png")]
    public class HeliumOptions
    {
        [Option("Disable Duplicant Operation", "If the box is checked, there's no need for a duplicant to come to work on the station.")]
        [JsonProperty]
        public bool DisableDuplicantOperation { get; set; }

        [Option("Power Consumption", "Electric power consumption of the machine. Default is 420W")]
        [JsonProperty]
        public float PowerConsumption { get; set; }

        [Option("Methane Conversion Per Seconds", "Mass (kg) of Methane filtered each second. Default is 0.5kg/s")]
        [JsonProperty]
        [Limit(0.1, 1)]
        public float MethaneConversionPerSeconds { get; set; }

        public HeliumOptions()
        {
            DisableDuplicantOperation = false;
            PowerConsumption = 420f;
            MethaneConversionPerSeconds = 0.5f;
        }
    }
}
