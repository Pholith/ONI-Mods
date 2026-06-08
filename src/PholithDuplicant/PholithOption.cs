using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace PholithDuplicant
{
    [JsonObject(MemberSerialization.OptIn), RestartRequired]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen1.png")]
    public class PholithOptions
    {
        [Option("Guarantee Pholith", "If the box is checked, Pholith is garanteed to spawn at each print.")]
        [JsonProperty]
        public bool GuaranteePholith { get; set; }

        [Option("Use Pholith real first name", "If the box is checked, use the real first name of Pholith as default name (Victoire).")]
        [JsonProperty]
        public bool UsePholithFirstName { get; set; }

        public PholithOptions()
        {
            GuaranteePholith = false;
            UsePholithFirstName = false;
        }
    }
}
