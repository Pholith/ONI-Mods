using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace Always3Interests
{
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public class Always3InterestsSettings
    {
        [Option("Number of interests", "The number of interests.")]
        [Limit(0, 6)]
        [JsonProperty]
        public int NumberOfInterests { get; set; }

        [Option("Random number of interests", "Active it to disable the interest modification.")]
        [JsonProperty]
        public bool RandomNumberOfInterests { get; set; }


        [Option("Points when 1 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhen1Interest { get; set; }

        [Option("Points when 2 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhen2Interest { get; set; }

        [Option("Points when 3 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhen3Interest { get; set; }

        [Option("Points when more than 3 interest", "")]
        [Limit(0, 50)]
        [JsonProperty]
        public int PointsWhenMoreThan3Interest { get; set; }

        [Option("Number of Good traits", "")]
        [Limit(0, 5)]
        [JsonProperty]
        public int NumberOfGoodTraits { get; set; }

        [Option("Number of Bad traits", "")]
        [Limit(0, 5)]
        [JsonProperty]
        public int NumberOfBadTraits { get; set; }

        [Option("Disable joy trait", "")]
        [JsonProperty]
        public bool DisableJoyTrait { get; set; }

        [Option("Disable stress trait", "")]
        [JsonProperty]
        public bool DisableStressTrait { get; set; }

        [Option("Starting level on printing pod", "Set the experience of in game printed dups.")]
        [Limit(0, 5)]
        [JsonProperty]
        public int StartingLevelOnPrintingPod { get; set; }

        [Option("Don't print biologic duplicants", "The printing pod will never show you biologic duplicants if this option is checked.\nWill not do anything if Bionic Booster Pack is not owned or activated on your save.", "Bionic Booster Pack")]
        [JsonProperty]
        public bool DisableBiologicDuplicants { get; set; }

        [Option("Don't print bionic duplicants", "The printing pod will never show you bionic duplicants if this option is checked.\nWill not do anything if Bionic Booster Pack is not owned or activated on your save.", "Bionic Booster Pack")]
        [JsonProperty]
        public bool DisableBionicDuplicants { get; set; }

        public Always3InterestsSettings()
        {
            PointsWhen1Interest = 9;
            PointsWhen2Interest = 5;
            PointsWhen3Interest = 1;

            PointsWhenMoreThan3Interest = 1;

            NumberOfInterests = 3;
            RandomNumberOfInterests = true;

            NumberOfGoodTraits = 1;
            NumberOfBadTraits = 1;
            DisableJoyTrait = false;
            DisableStressTrait = false;

            StartingLevelOnPrintingPod = 1;

            DisableBionicDuplicants = false;
            DisableBiologicDuplicants = false;
        }
    }
}
