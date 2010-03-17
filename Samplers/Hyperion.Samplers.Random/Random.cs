
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Samplers.Random
{
    public class Random : ISampler
    {
        private int nSamples;
        private int xPos;
        private int yPos;
        private int SamplePos;
        private double[] ImageSamples;
        private double[] LensSamples;
        private double[] TimeSamples;

        public Random (int xstart, int xend, int ystart, int yend, int ns, double sopen, double sclose) : base(xstart, xend, ystart, yend, ns, sopen, sclose)
        {
            nSamples = ns;
            xPos = xPixelStart;
            yPos = yPixelStart;

            ImageSamples = new double[2 * nSamples];
            LensSamples = new double[2 * nSamples];
            TimeSamples = new double[nSamples];

            for (int i = 0; i < nSamples; ++i)
            {
                TimeSamples[i] = Util.Random.NextDouble ();
            }

            for (int i = 0; i < 2 * nSamples; ++i)
            {
                LensSamples[i] = Util.Random.NextDouble ();
                ImageSamples[i] = Util.Random.NextDouble ();
            }

            for (int i = 0; i < 2 * nSamples; i += 2)
            {
                ImageSamples[i] += xPos;
                ImageSamples[i + 1] += yPos;
            }

            SamplePos = 0;
        }

        public override int GetMoreSamples (Sample[] sample)
        {
            if (SamplePos == nSamples)
            {
                if (xPixelStart == xPixelEnd || yPixelStart == yPixelEnd)
                    return 0;
                if (++xPos == xPixelEnd)
                {
                    xPos = xPixelStart;
                    ++yPos;
                }
                if (yPos == yPixelEnd)
                    return 0;
                
                for (int i = 0; i < nSamples; ++i)
                {
                    TimeSamples[i] = Util.Random.NextDouble ();
                }
                
                for (int i = 0; i < 2 * nSamples; ++i)
                {
                    LensSamples[i] = Util.Random.NextDouble ();
                    ImageSamples[i] = Util.Random.NextDouble ();
                }
                
                for (int i = 0; i < 2 * nSamples; i += 2)
                {
                    ImageSamples[i] += xPos;
                    ImageSamples[i + 1] += yPos;
                }

                SamplePos = 0;
            }
            // Return next \mono{RandomSampler} sample point
            sample[0].ImageX = ImageSamples[2 * SamplePos];
            sample[0].ImageY = ImageSamples[2 * SamplePos + 1];
            sample[0].LensU = LensSamples[2 * SamplePos];
            sample[0].LensV = LensSamples[2 * SamplePos + 1];
            sample[0].Time = Util.Lerp (TimeSamples[SamplePos], ShutterOpen, ShutterClose);
            // Generate stratified samples for integrators
            for (int i = 0; i < sample[0].n1D.Count; ++i)
                for (int j = 0; j < sample[0].n1D[i]; ++j)
                    sample[0].samples[sample[0].oneD + i][j] = Util.Random.NextDouble ();
            for (int i = 0; i < sample[0].n2D.Count; ++i)
                for (int j = 0; j < 2 * sample[0].n2D[i]; ++j)
                    sample[0].samples[sample[0].twoD + i][j] = Util.Random.NextDouble ();
            ++SamplePos;
            return 1;
        }

        public override ISampler GetSubSampler (int num, int count)
        {
            int x0, x1, y0, y1;
            ComputeSubWindow (num, count, out x0, out x1, out y0, out y1);
            if (x0 == x1 || y0 == y1)
                return null;
            return new Random (x0, x1, y0, y1, nSamples, ShutterOpen, ShutterClose);
        }

        public override bool ReportResults (Sample[] samples, Hyperion.Core.Geometry.RayDifferential[] rays, Spectrum[] Ls, Intersection[] isects, int count)
        {
            return base.ReportResults (samples, rays, Ls, isects, count);
        }

        public override int RoundSize (int size)
        {
            return size;
        }

        public override int MaximumSampleCount {
            get {
                return 1;
            }
        }

        public static ISampler CreateSampler (ParameterSet parameters, IFilm film, ICamera camera)
        {
            int ns = parameters.FindOneInt ("nsamples", 4);
            int xstart, xend, ystart, yend;
            film.GetSampleExtent (out xstart, out xend, out ystart, out yend);
            return new Random (xstart, xend, ystart, yend, ns, camera.ShutterOpen, camera.ShutterClose);
        }
    }
}
