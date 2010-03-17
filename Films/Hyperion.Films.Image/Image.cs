
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Parallel;
using Hyperion.Core.Tools;

namespace Hyperion.Films.Image
{
    public class Image : IFilm
    {
        internal class Pixel
        {
            public double[] Lxyz = new double[3];
            public double WeightSum;
            public double[] SplatXyz = new double[3];
            public double Pad;
        }

        private IFilter Filter;
        private int xPixelStart;
        private int xPixelCount;
        private int yPixelStart;
        private int yPixelCount;
        private double[] CropWindow;
        private string FileName;
        private double[] FilterTable;
        private Pixel[] Pixels;

        public Image (int xres, int yres, IFilter filter, double[] crop, string filename, bool openWindow) : base(xres, yres)
        {
            Filter = filter;
            CropWindow = crop;
            FileName = filename;
            xPixelStart = Util.Ceil2Int (xResolution * CropWindow[0]);
            xPixelCount = Math.Max (1, Util.Ceil2Int (xResolution * CropWindow[1]) - xPixelStart);
            yPixelStart = Util.Ceil2Int (yResolution * CropWindow[2]);
            yPixelCount = Math.Max (1, Util.Ceil2Int (yResolution * CropWindow[3]) - yPixelStart);

            Pixels = new Pixel[xPixelCount * yPixelCount];

            for (int i = 0; i < xPixelCount * yPixelCount; ++i)
                Pixels[i] = new Pixel ();

            FilterTable = new double[16 * 16];
            int index = 0;

            for (int y = 0; y < 16; ++y)
            {
                double fy = (y + 0.5) * Filter.yWidth / 16;

                for (int x = 0; x < 16; ++x)
                {
                    double fx = (x + 0.5) * Filter.xWidth / 16;
                    FilterTable[index++] = Filter.Evaluate (fx, fy);
                }
            }
        }

        public override void AddSample (CameraSample sample, Spectrum L)
        {
            double dImageX = sample.ImageX - 0.5;
            double dImageY = sample.ImageY - 0.5;
            int x0 = Util.Ceil2Int (dImageX - Filter.xWidth);
            int x1 = Util.Floor2Int (dImageX + Filter.xWidth);
            int y0 = Util.Ceil2Int (dImageY - Filter.yWidth);
            int y1 = Util.Floor2Int (dImageY + Filter.yWidth);
            x0 = Math.Max (x0, xPixelStart);
            x1 = Math.Min (x1, xPixelStart + xPixelCount - 1);
            y0 = Math.Max (y0, yPixelStart);
            y1 = Math.Min (y1, yPixelStart + yPixelCount - 1);

            if ((x1 - x0) < 0 || (y1 - y0) < 0)
                return;

            double[] xyz = new double[3];
            L.ToXyz (xyz);

            int[] ifx = new int[x1 - x0 + 1];
            for (int x = x0; x <= x1; ++x)
            {
                double fx = Math.Abs ((x - dImageX) * Filter.invXWidth * 16);
                ifx[x - x0] = Math.Min (Util.Floor2Int (fx), 15);
            }

            int[] ify = new int[y1 - y0 + 1];
            for (int y = y0; y <= y1; ++y)
            {
                double fy = Math.Abs ((y - dImageY) * Filter.invYWidth * 16);
                ify[y - y0] = Math.Min (Util.Floor2Int (fy), 15);
            }

            bool syncNeeded = (Filter.xWidth > 0.5 || Filter.yWidth > 0.5);
            for (int y = y0; y <= y0; ++y)
            {
                for (int x = x0; x <= x0; ++x)
                {
                    int offset = ify[y - y0] * 16 + ifx[x - x0];
                    double filterWeight = FilterTable[offset];

                    Pixel pixel = Pixels[(y - yPixelStart) * xPixelCount + (x - xPixelStart)];
                    if (!syncNeeded)
                    {
                        pixel.Lxyz[0] += filterWeight * xyz[0];
                        pixel.Lxyz[1] += filterWeight * xyz[1];
                        pixel.Lxyz[2] += filterWeight * xyz[2];
                        pixel.WeightSum += filterWeight;
                    }
                    else
                    {
                        ParallelUtility.AtomicAdd (ref pixel.Lxyz[0], filterWeight * xyz[0]);
                        ParallelUtility.AtomicAdd (ref pixel.Lxyz[1], filterWeight * xyz[1]);
                        ParallelUtility.AtomicAdd (ref pixel.Lxyz[1], filterWeight * xyz[2]);
                        ParallelUtility.AtomicAdd (ref pixel.WeightSum, filterWeight);
                    }
                }
            }
        }

        public override void Splat (CameraSample sample, Spectrum L)
        {
            if (L.HasNaNs)
                return;

            double[] xyz = new double[3];
            L.ToXyz (xyz);
            int x = Util.Floor2Int (sample.ImageX), y = Util.Floor2Int (sample.ImageY);
            if (x < xPixelStart || x - xPixelStart >= xPixelCount ||
                y < yPixelStart || y - yPixelStart >= yPixelCount)
                return;
            Pixel pixel = Pixels[(y - yPixelStart) * xPixelCount + (x - xPixelStart)];
            ParallelUtility.AtomicAdd (ref pixel.SplatXyz[0], xyz[0]);
            ParallelUtility.AtomicAdd (ref pixel.SplatXyz[1], xyz[1]);
            ParallelUtility.AtomicAdd (ref pixel.SplatXyz[2], xyz[2]);
        }

        public override void GetSampleExtent (out int xstart, out int xend, out int ystart, out int yend)
        {
            xstart = Util.Floor2Int (xPixelStart + 0.5 - Filter.xWidth);
            xend = Util.Floor2Int (xPixelStart + 0.5 + xPixelCount + Filter.xWidth);
            ystart = Util.Floor2Int (yPixelStart + 0.5 - Filter.yWidth);
            yend = Util.Floor2Int (yPixelStart + 0.5 + yPixelCount + Filter.yWidth);
        }

        public override void GetPixelExtent (out int xstart, out int xend, out int ystart, out int yend)
        {
            xstart = xPixelStart;
            xend = xPixelStart + xPixelCount;
            ystart = yPixelStart;
            yend = yPixelStart + yPixelCount;
        }

        public override void WriteImage (double splatScale)
        {
            Console.WriteLine (" > Saving image to {0}", FileName);
        }

        public override void UpdateDisplay (int x0, int y0, int x1, int y1, double splatScale)
        {
            base.UpdateDisplay (x0, y0, x1, y1, splatScale);
        }

        public static IFilm CreateFilm (ParameterSet parameters, IFilter filter)
        {
            string filename = parameters.FindOneString ("filename", "");
            if (filename == "")
                filename = "hyperion.tga";

            int xres = parameters.FindOneInt ("xresolution", 640);
            int yres = parameters.FindOneInt ("yresolution", 480);
            //if (PbrtOptions.quickRender) xres = max(1, xres / 4);
            //if (PbrtOptions.quickRender) yres = max(1, yres / 4);
            bool openwin = parameters.FindOneBool ("display", false);
            double[] crop = new double[] { 0, 1, 0, 1 };
            int cwi = 0;
            double[] cr = parameters.FindDouble ("cropwindow", ref cwi);
            if (cr != null && cwi == 4)
            {
                crop[0] = Util.Clamp (Math.Min (cr[0], cr[1]), 0.0, 1.0);
                crop[1] = Util.Clamp (Math.Max (cr[0], cr[1]), 0.0, 1.0);
                crop[2] = Util.Clamp (Math.Min (cr[2], cr[3]), 0.0, 1.0);
                crop[3] = Util.Clamp (Math.Max (cr[2], cr[3]), 0.0, 1.0);
            }

            return new Image (xres, yres, filter, crop, filename, openwin);
        }
    }
}
