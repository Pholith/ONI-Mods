using Newtonsoft.Json;
using PeterHan.PLib.Options;
using MemberSerialization = Newtonsoft.Json.MemberSerialization;

namespace HDScreenShot
{
    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("https://github.com/Pholith/ONI-Mods", "screen1.png")]
    public class HDScreenshotOptions
    {
        public enum ImageFormat
        {
            png,
            jpg
        }

        [Option("HDScreenShot.HD_STRINGS.OPTIONS.WIDTH", "HDScreenShot.HD_STRINGS.OPTIONS.WIDTH_TOOLTIP")]
        [JsonProperty]
        public int ScreenshotWidth { get; set; }

        [Option("HDScreenShot.HD_STRINGS.OPTIONS.HEIGHT", "HDScreenShot.HD_STRINGS.OPTIONS.HEIGHT_TOOLTIP")]
        [JsonProperty]
        public int ScreenshotHeight { get; set; }

        [Option("HDScreenShot.HD_STRINGS.OPTIONS.FORMAT")]
        [JsonProperty]
        public ImageFormat SavedImageFormat { get; set; }

        [Option("HDScreenShot.HD_STRINGS.OPTIONS.HIDE_TIPS")]
        [JsonProperty]
        public bool HideTipsTextOnScreenshot { get; set; }

        public HDScreenshotOptions()
        {
            ScreenshotWidth = 6144;
            ScreenshotHeight = 8192;
            SavedImageFormat = ImageFormat.jpg;
            HideTipsTextOnScreenshot = false;
        }
    }
}

