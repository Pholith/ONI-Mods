using Harmony;
using STRINGS;

namespace ILoveSlicksters
{
    class StringsPatch
    {

        public static class VARIANT_ETHANOL
        {

            public static LocString NAME = UI.FormatAsLink("Ethanol Slickster", "ETHANOLOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Ehtanol Slicksters are slimy critters that consume ",
                UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
                " and exude ",
                UI.FormatAsLink("Ethanol", "ETHANOL"),
                "."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Ethanol Slickster Egg", "ETHANOLOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Ethanol Larva", "ETHANOLOILFLOATER");

                public static LocString DESC = "A goopy little Ethanol Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Ethanol Slickster", "ETHANOLOILFLOATER") + ".";
            }

        }
        public static class VARIANT_OWO
        {

            public static LocString NAME = UI.FormatAsLink("OwO Slickster", "OWO_OILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "OwO Slicksters are slimy critters that consume ",
                UI.FormatAsLink("Oxygen", "OXYGEN"),
                " and exude ",
                UI.FormatAsLink("Hydrogen", "HYDROGEN"),
                "."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("OwO Slickster Egg", "OWO_OILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("OwO Larva", "OWO_OILFLOATER");

                public static LocString DESC = "A goopy little OwO Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("OwO Slickster", "OWO_OILFLOATER") + ".";
            }

        }

        public static class VARIANT_LEAFY
        {

            public static LocString NAME = UI.FormatAsLink("Leafy Slickster", "LEAFYOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Leafy Slicksters are slimy critters that consume ",
                UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
                " and exude ",
                UI.FormatAsLink("Oxygen", "OXYGEN"),
                ".\nLeafy Slicksters need light to live."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Leafy Slickster Egg", "LEAFYOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Leafy Larva", "LEAFYOILFLOATER");

                public static LocString DESC = "A goopy little Leafy Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Leafy Slickster", "LEAFYOILFLOATER") + ".";
            }

        }
        public static class VARIANT_ROBOT
        {

            public static LocString NAME = UI.FormatAsLink("Robot Slickster", "ROBOTOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Robot Slicksters are slimy critters that consume ",
                UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
                " and exude ",
                UI.FormatAsLink("Steel", "STEEL"),
                "."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Robot Slickster Egg", "ROBOTOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Robot Larva", "ROBOTOILFLOATER");

                public static LocString DESC = "A goopy little Robot Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Robot Slickster", "ROBOTOILFLOATER") + ".";
            }

        }
        public static class VARIANT_POLAR
        {

            public static LocString NAME = UI.FormatAsLink("Polar Slickster", "POLAROILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Polar Slicksters are slimy critters that consume ",
                UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
                " and exude ",
                UI.FormatAsLink("Ice", "ICE"),
                "."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Polar Slickster Egg", "POLAROILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Polar Larva", "POLAROILFLOATER");

                public static LocString DESC = "A goopy little Polar Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Polar Slickster", "POLAROILFLOATER") + ".";
            }

        }

        public static class FERTILITY_MODIFIERS
        {
            public static class LIGHT
            {
                public static LocString NAME = "Light";

                public static LocString DESC = "Is in the light";
            }
        }
    }
}