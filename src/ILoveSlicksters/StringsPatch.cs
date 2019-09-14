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
                ".\n\nThe ethanol slickster has adapted to a cold environment rich in carbon dioxide unlike its cousin the hairy slickster."
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
                ".\n\nThe OwO Slicksters are extremely cute creatures that will delight your duplicants."
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
                ".\n\nThe leafy slickster has adapted to its environment and uses photosynthesis to feed on carbon.\nLeafy Slicksters need light to live."
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
                ".\n\nThe robot slickster is a slime that has been technologically improved by Gravitas to meet our heavy metal needs."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Robot Slickster Egg", "ROBOTOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Robot Larva", "ROBOTOILFLOATER");

                public static LocString DESC = "A goopy little Robot Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Robot Slickster", "ROBOTOILFLOATER") + ".";
            }

        }
        public static class VARIANT_FROZEN
        {

            public static LocString NAME = UI.FormatAsLink("Frozen Slickster", "FROZENOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Frozen Slicksters are slimy critters that consume ",
                UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
                " and exude ",
                UI.FormatAsLink("Antigel", "ANTIGEL"),
                ".\n\nThe Frozen slickster is a distant variant of the slickster in a very cold environment, it produces a liquid appreciated by our engineers."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Frozen Slickster Egg", "FROZENOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Frozen Larva", "FROZENOILFLOATER");

                public static LocString DESC = "A goopy little Frozen Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Frozen Slickster", "FROZENOILFLOATER") + ".";
            }
        }

        public static class FERTILITY_MODIFIERS
        {
            public static class LIGHT
            {
                public static LocString NAME = "Light";

                public static LocString DESC = "Is in the light.";
            }
            public static class PRESSURE
            {
                public static LocString NAME = "Pressure";

                public static LocString DESC = "Is in air pressure below {0} g.";
            }
            public static class ELEMENT
            {
                public static LocString NAME = "Element";

                public static LocString DESC = "Is immersed in {0}.";
            }
        }

        public static class WORLDGEN
        {
            public static LocString NAME = "Slicksteria";
            public static LocString DESC = "";
        }
    }
}