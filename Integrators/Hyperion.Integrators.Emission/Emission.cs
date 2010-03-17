
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Integrators.Emission
{
    public class Emission : IVolumeIntegrator
    {
        public Emission ()
        {
        }

        public override Core.Spectrum Li (Scene scene, IRenderer renderer, RayDifferential ray, Sample sample, ref Spectrum transmittance)
        {
            return new Core.Spectrum ();
        }

        public override Core.Spectrum Transmittance (Scene scene, IRenderer renderer, RayDifferential ray, Sample sample)
        {
            return new Core.Spectrum ();
        }

        public override void RequestSamples (ISampler sampler, Sample sample, Scene scene)
        {
            base.RequestSamples (sampler, sample, scene);
        }

        public static IVolumeIntegrator CreateVolumeIntegrator (ParameterSet parameters)
        {
            double stepSize = parameters.FindOneDouble ("stepsize", 1.0);
            return new Emission ();
        }
    }
}
