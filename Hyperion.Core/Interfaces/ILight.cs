
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public abstract class ILight
    {
        protected Transform LightToWorld;
        protected Transform WorldToLight;
        public readonly int NumberOfSamples;

        public ILight (Transform lightToWorld) : this(lightToWorld, 1)
        {
        }

        public ILight (Transform lightToWorld, int ns)
        {
            NumberOfSamples = ns;
            LightToWorld = new Transform (lightToWorld);
            WorldToLight = LightToWorld.Inverse;
        }

        public virtual Spectrum Le (RayDifferential rd)
        {
            return new Spectrum ();
        }

        public virtual void SHProject (Point p, double pEpsilon, int lmax, Scene scene, bool computeLightVisibility, double time, Spectrum[] coeffs)
        {
        }

        public abstract Spectrum SampleL (Point p, double pEpsilon, LightSample ls, double time, ref Vector wi, ref double pdf, ref VisibilityTester visibility);
        public abstract Spectrum Power (Scene scene);
        public abstract double Pdf (Point p, Vector wi);
        public abstract Spectrum SampleL (Scene scene, LightSample ls, double u1, double u2, double time, ref Ray ray, ref Normal Ns, ref double pdf);
        public abstract bool IsDeltaLight { get; }
    }
}
