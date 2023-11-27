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

        [Option("Screenshot width", "Width of the screenshot in pixels.")]
        [JsonProperty]
        public int ScreenshotWidth { get; set; }

        [Option("Screenshot height", "Height of the screenshot in pixels.")]
        [JsonProperty]
        public int ScreenshotHeight { get; set; }

        [Option("Screenshot image format")]
        [JsonProperty]
        public ImageFormat SavedImageFormat { get; set; }

        public HDScreenshotOptions()
        {
            ScreenshotWidth = 6144;
            ScreenshotHeight = 8192;
            SavedImageFormat = ImageFormat.jpg;
        }
    }
}

