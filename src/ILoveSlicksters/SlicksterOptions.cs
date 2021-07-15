using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;

namespace ILoveSlicksters
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen1.png")]
    [RestartRequired]
    public class SlicksterOptions
    {
        [Option("Increases vanilla slicksters consumption", "If the box is ticked, the vanilla slicksters will consume 60kg/cycle instead of 20kg/cycle.\nIt doesn't affect new slicksters.")]
        [JsonProperty]
        public bool IncreasesVanillaSlickstersConsumption { get; set; }

        [Option("New Slicksters consumption multiplier", "New slicksters consumption is based on 60kg/s (3x the normal consumption).\nYou can increase this consumption again. If you multiply by 2, the base consumption will be 120kg/cycle.\nIt doesn't affect vanilla slicksters.")]
        [JsonProperty]
        [Limit(1, 3)]
        public int ConsumptionMultiplier { get; set; }

        [Option("Disable new slicksters egg recipes", "If the box is ticked, new recipes to create eggs will not be added to the super refinery.")]
        [JsonProperty]
        public bool DisableSlickstersRecipes { get; set; }

        [Option("Disable slicksters care packages", "If the box is ticked, the printing pod will not show new slicksters eggs.")]
        [JsonProperty]
        public bool DisableSlickstersCarePackages { get; set; }

        [Option("Disable longhair slickster buff", "If the box is ticked, the longhair slickster will not have a buff from the mod.")]
        [JsonProperty]
        public bool DisableLonghairSlicksters { get; set; }

        [Option("Longhair element conversion", "Set the element the longhair slickster will produce. Don't works if you disabled the longhair buff.")]
        [JsonProperty]
        public LonghairElementList LonghairElement { get; set; }


        public SlicksterOptions()
        {
            IncreasesVanillaSlickstersConsumption = true;
            ConsumptionMultiplier = 1;
            DisableSlickstersRecipes = false;
            DisableSlickstersCarePackages = false;
            DisableLonghairSlicksters = false;
            LonghairElement = LonghairElementList.Oxygen;
        }
    }
}

