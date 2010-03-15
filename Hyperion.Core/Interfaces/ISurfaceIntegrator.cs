
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{


    public abstract class ISurfaceIntegrator : IIntegrator
    {
        public abstract Spectrum Li (Scene scene, IRenderer renderer, RayDifferential ray, Intersection isect, Sample sample);
    }
}
