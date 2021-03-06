
using System;
using System.Drawing;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Tools
{
    public static class ImageIo
    {
        public static void WriteImage (string name, double[] pixels, double[] alpha, int xRes,
                int yRes, int totalXRes, int totalYRes, int xOffset, int yOffset)
        {
            WriteImagePNG (name, pixels, alpha, xRes, yRes, totalXRes, totalYRes, xOffset, yOffset);
        }

        public static void WriteImagePNG (string name, double[] pixels, double[] alpha, int xRes,
                int yRes, int totalXRes, int totalYRes, int xOffset, int yOffset)
        {
            Bitmap bitmap = new Bitmap (xRes, yRes);

            byte[] dest = new byte[3];
            for (int y = yRes - 1; y >= 0; --y)
            {
                for (int x = 0; x < xRes; ++x)
                {
                    dest[2] = (byte)Util.Clamp (255.0 * Math.Pow (pixels[3 * (y * xRes + x) + 2], 1.0 / 2.3), 0.0, 255.0);
                    dest[1] = (byte)Util.Clamp (255.0 * Math.Pow (pixels[3 * (y * xRes + x) + 1], 1.0 / 2.3), 0.0, 255.0);
                    dest[0] = (byte)Util.Clamp (255.0 * Math.Pow (pixels[3 * (y * xRes + x) + 0], 1.0 / 2.3), 0.0, 255.0);

                    bitmap.SetPixel (x, y, Color.FromArgb (dest[0], dest[1], dest[2]));
                }
            }
            bitmap.Save (name);
        }

        public static Spectrum[] ReadImage (string name, out int xSize, out int ySize)
        {
            Bitmap bitmap = new Bitmap (name);
            Spectrum[] pixels = new Spectrum[bitmap.Width * bitmap.Height];

            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    Color pixel = bitmap.GetPixel (x, y);
                    double[] c = new double[3];
                    c[0] = pixel.R / 255.0;
                    c[1] = pixel.G / 255.0;
                    c[2] = pixel.B / 255.0;
                    pixels[y * bitmap.Width + x] = new Spectrum (c[0], c[1], c[2]);
                }
            }

            xSize = bitmap.Width;
            ySize = bitmap.Height;

            return pixels;
        }
    }
}
