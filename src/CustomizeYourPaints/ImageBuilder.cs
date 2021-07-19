using HarmonyLib;
using Pholib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SolarSystemWorlds
{
    public class ImageBuilder
    {

        public static void OnLoad()
        {
            try
            {
                Image squarePainting = Image.FromFile(ImageUtil.ModPath() + "src/originals/painting_0_original.png");
                Image tallPainting = Image.FromFile(ImageUtil.ModPath() + "src/originals/painting_tall_0_original.png");
                Image widePainting = Image.FromFile(ImageUtil.ModPath() + "src/originals/painting_wide_0_original.png");

                // x and y position on the original image
                int[] paintingPositionsX = new int[] { 395, 390, 390, 532, 530 };
                int[] paintingPositionsY = new int[] { 565, 710, 860, 706, 868 };

                for (int i = 0; i < paintingPositionsX.Length; i++)
                {
                    try
                    {
                        squarePainting = ImageUtil.MergeImage(squarePainting, Image.FromFile(ImageUtil.ModPath() + "src/painting" + (i + 1) + ".png").GetThumbnailImage(120, 130, null, IntPtr.Zero), paintingPositionsX[i], paintingPositionsY[i]);
                    }
                    catch (Exception)
                    {
                        Logs.Log("File painting" + (i + 1) + ".png not found.");
                    }
                }
                int[] tallPaintingPositionsX = new int[] { 399, 558, 395, 555, 395, 397 };
                int[] tallPaintingPositionsY = new int[] { 15, 8, 254, 259, 503, 752 };

                for (int i = 0; i < tallPaintingPositionsX.Length; i++)
                {
                    try
                    {
                        tallPainting = ImageUtil.MergeImage(tallPainting, Image.FromFile(ImageUtil.ModPath() + "src/tall_painting" + (i + 1) + ".png").GetThumbnailImage(140, 230, null, IntPtr.Zero), tallPaintingPositionsX[i], tallPaintingPositionsY[i]);
                    }
                    catch (Exception)
                    {
                        Logs.Log("File tall_painting" + (i + 1) + ".png not found.");
                    }
                }
                int[] widePaintingPositionsX = new int[] { 265, 18, 255, 8, 255, 485 };
                int[] widePaintingPositionsY = new int[] { 424, 431, 570, 745, 747, 744 };
                for (int i = 0; i < widePaintingPositionsX.Length; i++)
                {
                    try
                    {
                        widePainting = ImageUtil.MergeImage(widePainting, Image.FromFile(ImageUtil.ModPath() + "src/wide_painting" + (i + 1) + ".png").GetThumbnailImage(215, 135, null, IntPtr.Zero), widePaintingPositionsX[i], widePaintingPositionsY[i]);
                    }
                    catch (Exception)
                    {
                        Logs.Log("File wide_painting" + (i + 1) + ".png not found.");
                    }
                }

                if (squarePainting != null)
                {
                    squarePainting.Save(ImageUtil.ModPath() + "anim/paints/mod_painting/painting_0.png", ImageFormat.Png);
                }

                if (tallPainting != null)
                {
                    tallPainting.Save(ImageUtil.ModPath() + "anim/paints/mod_painting_tall/painting_tall_0.png", ImageFormat.Png);
                }

                if (widePainting != null)
                {
                    widePainting.Save(ImageUtil.ModPath() + "anim/paints/mod_painting_wide/painting_wide_0.png", ImageFormat.Png);
                }
            }
            catch (IOException e)
            {
                Logs.Log("Error while loading painting images. " + e.ToString());
            }
            catch (Exception e)
            {
                Logs.Log("Unknown error while loading painting images. " + e.ToString());
            }

        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class DbPatch
    {
        public static void Postfix()
        {

            Dictionary<HashedString, KAnimFile> table = Traverse.Create<Assets>().Field("AnimTable").GetValue<Dictionary<HashedString, KAnimFile>>();

            table["painting_kanim"] = Assets.GetAnim("mod_painting_kanim");
            table["painting_tall_kanim"] = Assets.GetAnim("mod_painting_tall_kanim");
            table["painting_wide_kanim"] = Assets.GetAnim("mod_painting_wide_kanim");

        }
    }
}
