using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace PholithDuplicant
{
    [JsonObject(MemberSerialization.OptIn), RestartRequired]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen1.png")]
    public class PholithOptions
    {
        [Option("Guarantee Pholith", "If the box is checked, ")]
        [JsonProperty]
        public bool GuaranteePholith { get; set; }

        [Option("Guarantee Pholith", "If the box is checked, ")]
        [JsonProperty]
        public bool GuaranteePholith2 { get; set; }

        public PholithOptions()
        {
            GuaranteePholith = true;
            GuaranteePholith2 = false;
        }
    }
}
