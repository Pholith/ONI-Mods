using STRINGS;

namespace WallPumps
{
    public static class WALLPUMP_STRINGS
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class FAIRGASWALLPUMP
                {
                    public static LocString NAME = "Gas wall pump";
                    public static LocString DESC = "A gas pump that's also a wall";
                    public static LocString EFFECT = "Pumps out " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from a room";
                }
                public static class FAIRLIQUIDWALLPUMP
                {
                    public static LocString NAME = "Liquid wall pump";
                    public static LocString DESC = "A liquid pump that's also a wall";
                    public static LocString EFFECT = "Pumps out " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " from a room";
                }
                public static class FAIRGASWALLVENT
                {
                    public static LocString NAME = "Gas wall vent";
                    public static LocString DESC = "A gas vent that's also a wall";
                    public static LocString EFFECT = "Releases " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " into a room";
                }
                public static class VENTHIGHPRESSURE
                {
                    public static LocString NAME = "High pressure gas wall vent";
                    public static LocString DESC = "A high pressure gas vent that's also a wall";
                    public static LocString EFFECT = "Releases high pressure " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " into a room";
                }
                public static class FAIRLIQUIDWALLVENT
                {
                    public static LocString NAME = "Liquid wall vent";
                    public static LocString DESC = "A liquid vent that's also a wall";
                    public static LocString EFFECT = "Releases " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " into a room";
                }
            }
        }
        public static class UI_ADD
        {
            public static LocString TILES_INSULATED = "Make pumps and evacuations insulated";

            public static LocString ENABLED = "Enabled";

            public static LocString MAX_PRESSURE = "Max pressure";

            public static LocString PUMP_RATE = "Pump rate";

            public static LocString ENERGY_CONSUMPTION = "Energy consumption";

            public static LocString THERMAL_CONDUCTIVITY = "Thermal conductivity";
        }
    }


}
