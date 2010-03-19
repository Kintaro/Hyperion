
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Integrators.Emission
{
    public class Emission : IVolumeIntegrator
    {
        private double StepSize;
        private int TauSampleOffset;
        private int ScatterSampleOffset;

        public Emission (double stepSize)
        {
            StepSize = stepSize;
        }

        public override Spectrum Li (Scene scene, IRenderer renderer, RayDifferential ray, Sample sample, ref Spectrum transmittance)
        {
            transmittance = new Spectrum (1.0);
            return new Spectrum ();
        }

        public override Spectrum Transmittance (Scene scene, IRenderer renderer, RayDifferential ray, Sample sample)
        {
            if (scene.VolumeRegion == null)
                return new Spectrum (1.0);

            double step, offset;
            if (sample != null)
            {
                step = StepSize;
                offset = sample.samples[sample.oneD + TauSampleOffset][0];
            }
            else
            {
                step = 4.0 * StepSize;
                offset = Util.Random.NextDouble ();
            }
            /*Spectrum tau = scene.VolumeRegion.Tau (ray, step, offset);
            return (-tau).Exp;*/
            return null;
        }

        public override void RequestSamples (ISampler sampler, Sample sample, Scene scene)
        {
            TauSampleOffset = sample.Add1D (1);
            ScatterSampleOffset = sample.Add1D (1);
        }

        public static IVolumeIntegrator CreateVolumeIntegrator (ParameterSet parameters)
        {
            double stepSize = parameters.FindOneDouble ("stepsize", 1.0);
            return new Emission (stepSize);
        }
    }
}
