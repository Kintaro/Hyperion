
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Samplers.LowDiscrepancy
{
    public class LowDiscrepancy : ISampler
    {
        private int xPos, yPos, nPixelSamples;
        private double[] SampleBuffer;

        public LowDiscrepancy (int xstart, int xend, int ystart, int yend, int ps, double sopen, double sclose) : base(xstart, xend, ystart, yend, Util.RoundUpPow2 (ps), sopen, sclose)
        {
            xPos = xPixelStart;
            yPos = yPixelStart;
            
            nPixelSamples = Util.RoundUpPow2 (ps);
        }

        public override ISampler GetSubSampler (int num, int count)
        {
            int x0, x1, y0, y1;
            ComputeSubWindow (num, count, out x0, out x1, out y0, out y1);
            if (x0 == x1 || y0 == y1)
                return null;
            return new LowDiscrepancy (x0, x1, y0, y1, nPixelSamples, ShutterOpen, ShutterClose);
        }

        public override int GetMoreSamples (Sample[] sample)
        {
            if (yPos == yPixelEnd)
                return 0;
            if (SampleBuffer == null)
                SampleBuffer = new double[MonteCarlo.LowDiscrepancyPixelSampleDoublesNeeded (sample, nPixelSamples)];
            MonteCarlo.LowDiscrepancyPixelSample (xPos, yPos, ShutterOpen, ShutterClose, nPixelSamples, sample, SampleBuffer);
            
            if (++xPos == xPixelEnd)
            {
                xPos = xPixelStart;
                ++yPos;
            }
            
            return nPixelSamples;
        }

        public override int MaximumSampleCount {
            get { return nPixelSamples; }
        }

        public override int RoundSize (int size)
        {
            return Util.RoundUpPow2 (size);
        }

        public static ISampler CreateSampler (ParameterSet parameters, IFilm film, ICamera camera)
        {
            // Initialize common sampler parameters
            int xstart, xend, ystart, yend;
            film.GetSampleExtent (out xstart, out xend, out ystart, out yend);
            int nsamp = parameters.FindOneInt ("pixelsamples", 4);
            
            return new LowDiscrepancy (xstart, xend, ystart, yend, nsamp, camera.ShutterOpen, camera.ShutterClose);
        }
    }
}
