using STRINGS;

namespace ILoveSlicksters
{
    public static class PHO_STRINGS
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

                public static LocString DESC = "A goopy little Ethanol Larva.\n\nOne day it will grow into an adult morph, the " + UI.FormatAsLink("Ethanol Slickster", "ETHANOLOILFLOATER") + ".";
            }

        }
        public static class VARIANT_OWO
        {

            public static LocString NAME = UI.FormatAsLink("OwO Slickster", "OWO_OILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "OwO Slicksters are cute critters that consume ",
                UI.FormatAsLink("Oxygen", "OXYGEN"),
                " and exude ",
                UI.FormatAsLink("Hydrogen", "HYDROGEN"),
                ".\nThe OwO Slicksters are extremely cute creatures that will delight your duplicants.\n Duplicants passing near an OwO Slickster earn the ",
                UI.FormatAsLink("OwO effect", "OWOEFFECT"), " reducing the stress.\n",

            });

            public static LocString EGG_NAME = UI.FormatAsLink("OwO Slickster Egg", "OWO_OILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("OwO Larva", "OWO_OILFLOATER");

                public static LocString DESC = "A goopy little OwO Larva.\n\nOne day it will grow into an adult morph, the " + UI.FormatAsLink("OwO Slickster", "OWO_OILFLOATER") + ".";
            }

        }

        public static class VARIANT_LEAFY
        {

            public static LocString NAME = UI.FormatAsLink("Leafy Slickster", "LEAFYOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Leafy Slicksters are leafy critters that consume impur air.\n\nThe leafy slickster has adapted to its environment and uses photosynthesis to feed on carbon.\nLeafy Slicksters need light to live.\n\nLeafy slicksters are loved by duplicants for their air-depolluting properties, while providing a sweet ", UI.FormatAsLink("Floral Scent", "POLLENGERMS"),"."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Leafy Slickster Egg", "LEAFYOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Leafy Larva", "LEAFYOILFLOATER");

                public static LocString DESC = "A goopy little Leafy Larva.\n\nOne day it will grow into an adult morph, the " + UI.FormatAsLink("Leafy Slickster", "LEAFYOILFLOATER") + ".";
            }

        }
        public static class VARIANT_ROBOT
        {

            public static LocString NAME = UI.FormatAsLink("Robot Slickster", "ROBOTOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Robot Slicksters are slimy critters that consume hot ",
                UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"), ", ", UI.FormatAsLink("Steam", "STEAM"), " or ", UI.FormatAsLink("Sour Gas", "SOURGAS"),
                " and exude ",
                UI.FormatAsLink("Steel", "STEEL"),
                ".\n\nThe robot slickster is a slime that has been technologically improved by Gravitas to meet our heavy metal needs."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Robot Slickster Egg", "ROBOTOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Robot Larva", "ROBOTOILFLOATER");

                public static LocString DESC = "A goopy little Robot Larva.\n\nOne day it will grow into an adult morph, the " + UI.FormatAsLink("Robot Slickster", "ROBOTOILFLOATER") + ".";
            }

        }
        public static class VARIANT_FROZEN
        {

            public static LocString NAME = UI.FormatAsLink("Frozen Slickster", "FROZENOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Frozen Slicksters are slimy critters that consume air to reject ", UI.FormatAsLink("Water", "WATER"), " or ", UI.FormatAsLink("Antigel", "ANTIGEL"),
                ".\n\nThe Frozen slickster is a distant variant of the slickster in a very cold environment, it produces a liquid appreciated by our engineers."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Frozen Slickster Egg", "FROZENOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Frozen Larva", "FROZENOILFLOATER");

                public static LocString DESC = "A goopy little Frozen Larva.\n\nOne day it will grow into an adult morph, the " + UI.FormatAsLink("Frozen Slickster", "FROZENOILFLOATER") + ".";
            }
        }
        public static class VARIANT_AQUA
        {

            public static LocString NAME = UI.FormatAsLink("Aqua Slickster", "AQUAOILFLOATER");

            public static LocString DESC = string.Concat(new string[]
            {
                "Aqua Slicksters are aquatic critters that live in any water and consume any water.",
                "\n\nThe Aqua slickster is a prehistoric strain of slickster when it was still living underwater. It is appreciated by the duplicants because it requires little maintenance of its aquarium."
            });

            public static LocString EGG_NAME = UI.FormatAsLink("Aqua Slickster Egg", "AQUAOILFLOATER");

            public static class BABY
            {
                public static LocString NAME = UI.FormatAsLink("Aqua Larva", "AQUAOILFLOATER");

                public static LocString DESC = "A goopy little Aqua Larva.\n\nOne day it will grow into an adult morph, the " + UI.FormatAsLink("Aqua Slickster", "AQUAOILFLOATER") + ".";
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

        public static class INAQUARIUM
        {
            public static LocString NAME = "In an aquarium";
        }
        public static class NOTINAQUARIUM
        {
            public static LocString NAME = "Not in an aquarium";
        }


        public static class DROWNING
        {

            public static LocString NAME = "Drowning";

            public static LocString TOOLTIP = string.Concat(new string[]
            {
                    "This critter can't breathe in ",
                    UI.PRE_KEYWORD,
                    "Air",
                    UI.PST_KEYWORD,
                    "!"
            });
        }

        public static class ELEMENTS
        {
            public static class ANTIGEL
            {
                public static LocString NAME = UI.FormatAsLink("Antigel", Antigel.Id.ToUpper());
                public static LocString DESC = $"A mixture of water(H<sub>2</sub>O) and Ethylen Glycol (C<sub>2</sub>H<sub>6</sub>O<sub>2</sub>).\n\nIt has been designed by engineers to be a good heat transfer fluid that does not freeze or vaporize easily.";

            }
        }

        public static class WORLDGEN
        {
            public static LocString NAME = "Slicksteria";
            public static LocString DESC = "Slicksteria is an asteroid very close to Verdante but which has the particularity of containing a great diversity of slicksters, the conditions on this asteroid are perfect for their development and these small bugs will help your colony to survive.\n\n";
        }
    }
}