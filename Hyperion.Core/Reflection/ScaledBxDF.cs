
using System;
using Hyperion.Core;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class ScaledBxDF : BxDF
    {
        private BxDF bxdf;
        private Spectrum s;

        public ScaledBxDF (BxDF b, Spectrum sc) : base(b.Type)
        {
            bxdf = b;
            s = sc;
        }
        public override Spectrum Rho (Vector w, int nSamples, double[] samples)
        {
            return s * bxdf.Rho (w, nSamples, samples);
        }
        public override Spectrum Rho (int nSamples, double[] samples1, double[] samples2)
        {
            return s * bxdf.Rho (nSamples, samples1, samples2);
        }
        public override Spectrum F (Vector wo, Vector wi)
        {
            return s * bxdf.F (wo, wi);
        }
        public override Spectrum SampleF (Vector wo, ref Vector wi, double u1, double u2, ref double pdf)
        {
            return s * bxdf.SampleF (wo, ref wi, u1, u2, ref pdf);
        }
    }
}
