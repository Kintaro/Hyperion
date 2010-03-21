
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public abstract class IRenderer
    {
        public ICamera Camera;

        public abstract void Render (Scene scene);
        public abstract Spectrum Li (Scene scene, RayDifferential ray, Sample sample, ref Intersection isect, ref Spectrum T);
        public abstract Spectrum Transmittance (Scene scene, RayDifferential ray, Sample sample);

        public Spectrum Li (Scene scene, RayDifferential ray, Sample sample)
        {
            Spectrum T = new Spectrum ();
            Intersection isect = new Intersection ();
            return Li (scene, ray, sample, ref isect, ref T);
        }

        public Spectrum Li (Scene scene, RayDifferential ray, Sample sample, ref Intersection isect)
        {
            Spectrum T = new Spectrum ();
            return Li (scene, ray, sample, ref isect, ref T);
        }
    }
}
