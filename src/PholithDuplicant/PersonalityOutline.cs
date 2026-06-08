using Newtonsoft.Json;

namespace PholithDuplicant
{

    internal class PersonalityOutline
    {
        [JsonProperty]
        public bool Printable { get; set; } = true;

        [JsonProperty]
        public bool Randomize { get; set; }

        [JsonProperty]
        public bool StartingMinion { get; set; } = true;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PersonalityType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StressTrait { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JoyTrait { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StickerType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeadShape { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Neck { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Mouth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Eyes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Hair { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Body { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CongenitalTrait { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Belt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Cuff { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Foot { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Hand { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Pelvis { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Leg { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ArmSkin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LegSkin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool RoboMouthConversation { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SpeechMouth { get; set; }

        [JsonIgnore]
        private string RequiredDlcID { get; set; }
    }
}