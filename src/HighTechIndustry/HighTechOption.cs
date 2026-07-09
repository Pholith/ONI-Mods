using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace HighTechIndustry
{
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen1.png")]
    public class HighTechOption
    {

        [Option("Restore Helium true properties", "Restore Helium true heat-capacity and thermal conductivity (This is really powerfull and can be game-breaking).\n" +
    "Thermal Conductivity change from 0.236 to 0.15 (DUT/(m*s))/°C\n" +
    "Specific heat capacity change from 0.14 to 5.193 (DUT/g)/°C")]
        [JsonProperty]
        public bool RestoreHeliumTrueProperty { get; set; }

        public HighTechOption()
        {
            RestoreHeliumTrueProperty = false;
        }

    }
}
