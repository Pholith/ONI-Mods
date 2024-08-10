using static STRINGS.UI;

namespace GigaWattWire
{
    public class WIRE_STRINGS
    {

        public static class JACKETED_WIRE
        {
            public static LocString NAME = FormatAsLink("Insulated Wire", "JACKETEDWIRE");

            public static LocString EFFECT = string.Concat("Connects buildings to ", FormatAsLink("Power", "POWER"), " sources. \n\nCan be run through wall and floor tile.");

            public static LocString DESC = "By coating the wires in plastic they suddenly became much safer.";
        }

        public static class JACKETEDWIREBRIDGE
        {
            public static LocString NAME = FormatAsLink("Insulated Wire Bridge", "JACKETEDWIREBRIDGE");

            public static LocString EFFECT = string.Concat("Carries even more ", FormatAsLink("Wattage", "POWER"), " than a ", FormatAsLink("Conductive Wire Bridge", "WIREREFINEDBRIDGE"), " without overloading.");

            public static LocString DESC = "By coating the wires in plastic they suddenly became much safer.";
        }
        public static class MEGAWATTWIRE
        {
            public static LocString NAME = FormatAsLink("Mega-Watt Wire", "MEGAWATTWIRE");

            public static LocString EFFECT = string.Concat("Carries even more ", FormatAsLink("Wattage", "POWER"), " than a ", FormatAsLink("Jacked Wire", "WIREREFINEDHIGHWATTAGE"), " without overloading.\n\nCannot be run through wall and floor tile.");

            public static LocString DESC = "This one goes up to 1000 kW.";
        }
        public static class MEGAWATTWIREBRIDGE
        {
            public static LocString NAME = FormatAsLink("Mega-Watt Joint Plate", "MEGAWATTWIREBRIDGE");

            public static LocString EFFECT = string.Concat("Carries even more ", FormatAsLink("Wattage", "POWER"), " than a ", FormatAsLink("Jacked Wire", "WIREREFINEDBRIDGEHIGHWATTAGE"), " without overloading.\n\n",
                "Allows ", FormatAsLink(" Mega-Watt Wire", "MEGAWATTWIRE"), " to be run through wall and floor tile.");

            public static LocString DESC = "Joint plates can run Mega-Watt wires through walls without leaking gas or liquid.";
        }
        public static class GIGAWATTWIRE
        {
            public static LocString NAME = FormatAsLink("Giga-Watt Wire", "GIGAWATTWIRE");

            public static LocString EFFECT = string.Concat("Carries even more ", FormatAsLink("Wattage", "POWER"), " than a ", FormatAsLink("Mega - Watt Wire", "MEGAWATTWIRE"), ".\n\nCannot be run through wall and floor tile.");

            public static LocString DESC = "By using special superconductive materials and low temperatures this wire can carry up to 1000 MW.";
        }
        public static class GIGAWATTWIREBRIDGE
        {
            public static LocString NAME = FormatAsLink("Giga-Watt Joint Plate", "GIGAWATTWIREBRIDGE");

            public static LocString EFFECT = string.Concat("Carries even more ", FormatAsLink("Wattage", "POWER"), " than a ", FormatAsLink("Mega - Watt Conductive Joint Plate", "MEGAWATTWIREBRIDGE"), ".\n\n",
                "Allows ", FormatAsLink(" Mega-Watt Wire", "MEGAWATTWIRE"), " to be run through wall and floor tile.");

            public static LocString DESC = "Joint plates can run Giga-Watt wires through walls without leaking gas or liquid.";
        }
        public static class POWERTRANSFORMER100KW
        {
            public static LocString NAME = FormatAsLink("100 kW Power Transformer", "POWERTRANSFORMER100KW");

            public static LocString EFFECT = string.Concat("Limits ", FormatAsLink("Power", "POWER"), " flowing through the Transformer to 100 kW");

            public static LocString DESC = "For those times you need a really big transformer.";
        }

        public static class POWERTRANSFORMER2MW
        {
            public static LocString NAME = FormatAsLink("2 MW Power Transformer", "POWERTRANSFORMER2MW");

            public static LocString EFFECT = string.Concat("Limits ", FormatAsLink("Power", "POWER"), " flowing through the Transformer to 2 MW");

            public static LocString DESC = "For those times you need an even bigger transformer.";
        }

        public static class UI
        {
            public static class UNITSUFFIXES
            {
                public static class ELECTRICAL
                {
                    public static LocString MEGAWATT = "MW";
                    public static LocString GIGAWATT = "GW";
                    public static LocString TERAWATT = "TW";
                }
            }
        }

        public static class MISC
        {
            public static class TAGS
            {
                public static LocString CONDUCTOR = "Conductor";
                public static LocString TEMPCONDUCTORSOLID= "Extreme Conductor";
            }
            public static LocString CONDUCTOR = "Conductor";
        }
    }
}
