using Newtonsoft.Json;
using PeterHan.PLib;

namespace ILoveSlicksters
{
    public class SlicksterOptions
    {
        [Option("Increases vanilla slicksters consumption", "If the box is ticked, the vanilla slicksters will consume 60kg/cycle instead of 20kg/cycle.\nIt doesn't affect new slicksters.\nGame must be restarted after change.")]
        [JsonProperty]
        public bool increasesVanillaSlickstersConsumption { get; set; }

        public SlicksterOptions()
        {
            increasesVanillaSlickstersConsumption = true;
        }
    }
}

