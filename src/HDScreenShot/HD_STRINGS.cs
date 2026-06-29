using Pholib;

namespace HDScreenShot
{
    public static class HD_STRINGS
    {
        public static class UI
        {
            public static LocString BUTTON_TEXT = "Take a HD Screenshot";
            
            public static LocString SAVED_AT = "Screenshot saved at: \n";
            

            public static LocString WARNING = Utilities.FormatColored("Sometimes on the screenshot tiles may be blured or \"molting\".\nTo prevent that, before your screenshot: Press Alt+S and unzoom at maximum.\n", "adadad", false);

            public static LocString TIPS = Utilities.FormatColored("TIPS: To take a screenshot of a layer, select the layer in-game and then open the menu by clicking the button rather than pressing ‘ESC’.", "adadad", false);

            public static LocString CLOSE = "CLOSE";
        }

        public static class OPTIONS
        {
            public static LocString WIDTH = "Screenshot width";
            
            public static LocString WIDTH_TOOLTIP = "Width of the screenshot in pixels.";

            public static LocString HEIGHT = "Screenshot height";

            public static LocString HEIGHT_TOOLTIP = "Height of the screenshot in pixels.";
            
            public static LocString FORMAT = "Screenshot image format";

            public static LocString HIDE_TIPS = "Hide tips info on popup";

        }
    }
}
