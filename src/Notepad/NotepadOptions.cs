using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace Notepad
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen1.png")]
    public class NotepadOptions
    {
        [Option("Instant Build", "If the box is checked, there's no need for a duplicant to come and build the notepad. It builds itself instantly.")]
        [JsonProperty]
        public bool InstantBuild { get; set; }

        [Option("Number of text lines", "Number of lines in the notepad side screen.")]
        [JsonProperty]
        public int LineNumber { get; set; }

        public NotepadOptions()
        {
            InstantBuild = false;
            LineNumber = 6;
        }
    }
}

