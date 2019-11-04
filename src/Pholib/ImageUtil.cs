using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace Pholib
{
    public class ImageUtil
    {

        public static String ModPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar.ToString();
        }

        public static void BuildImage()
        {
            try
            {
                Image squarePainting = Image.FromFile(ModPath() + "src/originals/painting_0_original.png");
                Image tallPainting = Image.FromFile(ModPath() + "src/originals/painting_tall_0_original.png");
                Image widePainting = Image.FromFile(ModPath() + "src/originals/painting_wide_0_original.png");

                // x and y position on the original image
                int[] paintingPositionsX = new int[] { 395, 390, 390, 532, 530 };
                int[] paintingPositionsY = new int[] { 565, 710, 860, 706, 868 };

                for (int i = 0; i < paintingPositionsX.Length; i++)
                {
                    try
                    {
                        squarePainting = MergeImage(squarePainting, Image.FromFile(ModPath() + "src/painting" + (i + 1) + ".png").GetThumbnailImage(120, 130, null, IntPtr.Zero), paintingPositionsX[i], paintingPositionsY[i]);
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
                        tallPainting = MergeImage(tallPainting, Image.FromFile(ModPath() + "src/tall_painting" + (i + 1) + ".png").GetThumbnailImage(140, 230, null, IntPtr.Zero), tallPaintingPositionsX[i], tallPaintingPositionsY[i]);
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
                        widePainting = MergeImage(widePainting, Image.FromFile(ModPath() + "src/wide_painting" + (i + 1) + ".png").GetThumbnailImage(215, 135, null, IntPtr.Zero), widePaintingPositionsX[i], widePaintingPositionsY[i]);
                    }
                    catch (Exception)
                    {
                        Logs.Log("File wide_painting" + (i + 1) + ".png not found.");
                    }
                }

                if (squarePainting != null) squarePainting.Save(ModPath() + "anim/paints/mod_painting/painting_0.png", ImageFormat.Png);
                if (tallPainting != null) tallPainting.Save(ModPath() + "anim/paints/mod_painting_tall/painting_tall_0.png", ImageFormat.Png);
                if (widePainting != null) widePainting.Save(ModPath() + "anim/paints/mod_painting_wide/painting_wide_0.png", ImageFormat.Png);

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

        public static Image MergeImage(Image master, Image commit, int x, int y)
        {
            if (master == null) return null;
            if (commit == null) return master; // do nothing

            for (int i = x; i < x + commit.Width; i++)
            {
                for (int j = y; j < y + commit.Height; j++)
                {
                    if (j >= master.Height) continue; // error
                    if (i >= master.Width) continue; // error
                    ((Bitmap)master).SetPixel(i, j, ((Bitmap)commit).GetPixel(i-x, j-y));
                }
            }

            return master;
        }

        public static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            if (firstImage == null)
            {
                throw new ArgumentNullException("firstImage");
            }

            if (secondImage == null)
            {
                throw new ArgumentNullException("secondImage");
            }

            int outputImageWidth = firstImage.Width > secondImage.Width ? firstImage.Width : secondImage.Width;

            int outputImageHeight = firstImage.Height + secondImage.Height + 1;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                    new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(secondImage, new Rectangle(new Point(0, firstImage.Height + 1), secondImage.Size),
                    new Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }
    }
}
