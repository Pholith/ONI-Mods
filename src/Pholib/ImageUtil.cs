using System.IO;
using System.Reflection;
using UnityEngine;

namespace Pholib
{
    public static class ImageUtil
    {

        public static string ModPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar.ToString();
        }

        public static Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            } else
            {
                Logs.Error($"{filePath} not found !");
            }
            return tex;
        }

        /// <summary>
        /// Extension method to create a thumbnail from a texture2D
        /// </summary>
        public static Texture2D ScaleTexture(this Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }
            result.Apply();
            return result;
        }

        public static Texture2D MergeImage(Texture2D background, Texture2D watermark, int startPositionX, int startPositionY)
        {
            startPositionY = -startPositionY - watermark.height;
            //only read and rewrite the area of the image2
            for (int x = startPositionX; x < background.width; x++)
            {
                for (int y = startPositionY; y < background.height; y++)
                {
                    if (x - startPositionX < watermark.width && y - startPositionY < watermark.height)
                    {
                        var bgColor = background.GetPixel(x, y);
                        var wmColor = watermark.GetPixel(x - startPositionX, y - startPositionY);

                        var finalColor = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                        background.SetPixel(x, y, finalColor);
                    }
                }
            }

            background.Apply();
            return background;
        }
    }
}
