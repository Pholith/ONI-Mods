using STRINGS;

namespace PholithDuplicant
{
    public static class STRINGS
    {
        public static class ITEMS
        {
            public static class BIONIC_BOOSTERS
            {
                public static class BOOSTER_DEGAUSSER_COIL
                {
                    public static LocString NAME = UI.FormatAsLink("Degausser Coil Booster", PholithDuplicant.degausser_coil_id.ToUpperInvariant());
                    public static LocString DESC = "Overrides a Bionic Duplicant's safety protocols to reduce stress.\n\n<b>WARNING:</b>\nUsage reduce bionic system capacity.";

                }
            }
        }
    }
}
