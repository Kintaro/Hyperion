
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public abstract class ISampler
    {
        public readonly int xPixelStart;
        public readonly int xPixelEnd;
        public readonly int yPixelStart;
        public readonly int yPixelEnd;
        public readonly int SamplesPerPixel;
        public readonly double ShutterOpen;
        public readonly double ShutterClose;

        public ISampler (int xstart, int xend, int ystart, int yend, int spp, double sopen, double sclose)
        {
            xPixelStart = xstart;
            xPixelEnd = xend;
            yPixelStart = ystart;
            yPixelEnd = yend;
            SamplesPerPixel = spp;
            ShutterOpen = sopen;
            ShutterClose = sclose;
        }

        public abstract int GetMoreSamples (Sample[] sample);
        public abstract ISampler GetSubSampler (int num, int count);
        public abstract int RoundSize (int size);

        public abstract int MaximumSampleCount
        {
            get;
        }

        public virtual bool ReportResults (Sample[] samples, RayDifferential[] rays, Spectrum[] Ls, Intersection[] isects, int count)
        {
            return true;
        }

        protected void ComputeSubWindow (int num, int count, out int newXStart, out int newXEnd, out int newYStart, out int newYEnd)
        {
            int dx = xPixelEnd - xPixelStart, dy = yPixelEnd - yPixelStart;
            int nx = count, ny = 1;
            while ((nx & 0x1) == 0 && dx * nx > dy * ny)
            {
                nx >>= 1;
                ny <<= 1;
            }
            
            // Compute $x$ and $y$ pixel sample range for sub-window
            int xo = num % nx, yo = num / nx;
            double tx0 = (double)xo / (double)nx, tx1 = (double)(xo + 1) / (double)nx;
            double ty0 = (double)yo / (double)ny, ty1 = (double)(yo + 1) / (double)ny;
            newXStart = Util.Floor2Int (Util.Lerp (tx0, xPixelStart, xPixelEnd));
            newXEnd = Util.Floor2Int (Util.Lerp (tx1, xPixelStart, xPixelEnd));
            newYStart = Util.Floor2Int (Util.Lerp (ty0, yPixelStart, yPixelEnd));
            newYEnd = Util.Floor2Int (Util.Lerp (ty1, yPixelStart, yPixelEnd));
        }
    }
}
