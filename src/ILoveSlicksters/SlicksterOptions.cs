using Newtonsoft.Json;
using PeterHan.PLib;

namespace ILoveSlicksters
{
    public class SlicksterOptions
    {
        [Option("Increases vanilla slicksters consumption", "If the box is ticked, the vanilla slicksters will consume 60kg/cycle instead of 20kg/cycle.\nIt doesn't affect new slicksters.\nGame must be restarted after change.")]
        [JsonProperty]
        public bool IncreasesVanillaSlickstersConsumption { get; set; }

        [Option("New Slicksters consumption multiplier", "New slicksters consumption is based on 60kg/s (3x the normal consumption).\nYou can increase this consumption again. If you multiply by 2, the base consumption will be 120kg/cycle.\nIt doesn't affect vanilla slicksters.\nGame must be restarted after change.")]
        [JsonProperty]
        [Limit(1, 3)]
        public int ConsumptionMultiplier { get; set; }

        public SlicksterOptions()
        {
            IncreasesVanillaSlickstersConsumption = true;
            ConsumptionMultiplier = 1;
        }
    }
}

