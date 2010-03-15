
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{


    public abstract class IVolumeIntegrator : IIntegrator
    {
        public abstract Spectrum Li (Scene scene, IRenderer renderer, RayDifferential ray, Sample sample, ref Spectrum transmittance);
        public abstract Spectrum Transmittance (Scene scene, IRenderer renderer, RayDifferential ray, Sample sample);
    }
}
