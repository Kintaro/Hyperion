
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class Lambertian : BxDF
    {
        private Spectrum R;

        public Lambertian (Spectrum reflectance) : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE)
        {
            R = reflectance;
        }

        public override Spectrum F (Vector wo, Vector wi)
        {
            return R * Util.InvPi;
        }

        public override Spectrum Rho (int nSamples, double[] samples1, double[] samples2)
        {
            return R;
        }

        public override Spectrum Rho (Vector wo, int nSamples, double[] samples)
        {
            return R;
        }
    }
}
