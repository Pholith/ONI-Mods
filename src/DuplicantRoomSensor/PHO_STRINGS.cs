using STRINGS;

namespace DuplicantRoomSensor
{
    public static class PHO_STRINGS
    {
        public static class DUP_CRITTER_COUNT_SIDE_SCREEN
        {
            public static LocString TITLE = "Duplicant Count Sensor";

            public static LocString TOOLTIP_ABOVE = string.Concat(new string[]
            {
                    "Will send a ",
                    UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
                    " if there are more than <b>{0}</b> ",
                    UI.PRE_KEYWORD,
                    "Duplicants",
                    UI.PST_KEYWORD,
                    " in the room"
            });

            public static LocString TOOLTIP_BELOW = string.Concat(new string[]
            {
                    "Will send a ",
                    UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active),
                    " if there are fewer than <b>{0}</b> ",
                    UI.PRE_KEYWORD,
                    "Duplicants",
                    UI.PST_KEYWORD,
                    " in the room"
            });

            public static LocString START = "Turn On";
            public static LocString STOP = "Turn Off";
            public static LocString VALUE_NAME = "Duplicant Count";
        }
    }
}
