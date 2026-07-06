namespace HighTechIndustry
{
    public static class PHO_STRINGS
    {

        public static class NEUTRONIC_TRANSMUTATION_CHAMBER
        {
            public static LocString NAME = "Neutronic transmutation chamber";
            public static LocString DESC = "DESC";
            public static LocString EFFECT = "EFFECT";


            public static LocString PORT_NAME = "Operating";
            public static LocString PORT_ACTIVE = "This is active";
            public static LocString PORT_INACTIVE = "This is inactive";


        }

        public static class RECIPE
        {
            // Doc here https://en.wikipedia.org/wiki/Power-to-gas#Power-to-methane
            public static LocString POWER_TO_METHANE_NAME = "Power-to-methane";
            public static LocString POWER_TO_METHANE_DESC = "A power-to-methane system combines hydrogen from an electrolysis system with carbon dioxide to produce methane using a methanation reaction such as the Sabatier reaction:\r\n4H<sub>2</sub> + CO<sub>2</sub> → CH<sub>4</sub> + 2H<sub>2</sub>O.";

            public static LocString NUCLEAR_WASTE_RECYCLING_NAME = "Nuclear Waste Recycling";
            public static LocString NUCLEAR_WASTE_RECYCLING_DESC = "Nuclear Waste Recycling description";


            public static LocString HYDROGEN_TO_HELIUM = "Transmute <sup>2</sup>H to Helium by β− decay.";
            public static LocString CARBON_TO_OXYGEN = "Transmute <sup>12</sup>C to Oxygen by β− decay and filter Nitrogen.";
            public static LocString CARBON_TO_NITROGEN = "Transmute <sup>12</sup>C to Nitrogen by β− decay.";
            public static LocString NITROGEN_TO_OXYGEN = "Transmute <sup>14</sup>N to Oxygen by β− decay.";
            public static LocString SODIUM_TO_ALUMINIUM = "Transmute <sup>23</sup>Na (Sodium) to Aluminium by β− decay and filter Magnesium.";
            public static LocString ALUMINIUM_TO_PHOSPHORUS = "Transmute <sup>27</sup>Al (Aluminium) to Phosphorus and Silicon by β− decay.";
            public static LocString SILICON_TO_PHOSPHORUS = "Transmute Silicon in the sand to Phosphorus by β− decay. Results a bit of Sulfur in the process.";
            public static LocString SULFUR_TO_CHLORINE = "Transmute Sulfur to Chlorine by β− decay and filter residual Argon.";
            public static LocString IRON_TO_COBALT = "Transmute Iron to cobalt by β− decay. Results a bit of Nickel in the process.";
            public static LocString COBALT_TO_NICKEL = "Transmute <sup>59</sup>Co (Cobalt) to Nickel by β− decay.";
            public static LocString NICKEL_TO_COPPER = "Transmute Nickel to Copper by β− decay. Results a bit of Zinc in the process.";
            public static LocString COPPER_TO_NICKEL_AND_ZINC = "Transmute <sup>63</sup>Cu (Copper) to <sup>62</sup>Ni (Nickel) by β+ decay. Some <sup>65</sup>Cu will transmute to Zn<sup>66</sup> by β+ decay.";
            public static LocString ZINC_TO_COPPER = "Transmute <sup>64</sup>Zn (zinc) to <sup>63</sup>Cu (Copper) by β+ decay and filter Galium.";
            public static LocString TUNGSTENE_TO_IRIDIUM = "Transmute Tungstene to Iridium by multiple β+ decay and filter a lot of unwanted outputs.";
            public static LocString MERCURY_TO_GOLD = "Transmute extra rare <sup>196</sup>Hg (Mercury) to Gold by β+ decay and filter some Platinium. Results contains others Mercury isotopes and Lead.";
            public static LocString MERCURY_TO_LEAD = "Transmute Mercury to Lead by β- decay.";
        }
    }
}
