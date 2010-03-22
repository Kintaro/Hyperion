
using System;
using Hyperion.Core;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class Microfacet : BxDF
    {
        private Spectrum R;
        private IMicrofacetDistribution Distribution;
        private IFresnel Fresnel;

        public Microfacet (Spectrum reflectance, IFresnel fresnel, IMicrofacetDistribution distribution) : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_GLOSSY)
        {
            R = new Spectrum (reflectance.c[0], reflectance.c[1], reflectance.c[2]);
            Fresnel = fresnel;
            Distribution = distribution;
        }

        public override Spectrum F (Vector wo, Vector wi)
        {
            double cosTheta0 = Math.Abs (Util.CosTheta (wo));
            double cosTheta1 = Math.Abs (Util.CosTheta (wi));
            Vector wh = (wi + wo).Normalized;
            double cosThetaH = (wi ^ wh);
            Spectrum F = Fresnel.Evaluate (cosThetaH);
            return R * Distribution.D (wh) * G (wo, wi, wh) * F / (4.0 * cosTheta1 * cosTheta0);
        }

        public override Spectrum SampleF (Vector wo, ref Vector wi, double u1, double u2, ref double pdf)
        {
            Distribution.SampleF (wo, ref wi, u1, u2, ref pdf);
            if (!Util.SameHemisphere (wo, wi))
                return new Spectrum (0.0);
            return F (wo, wi);
        }

        public override double Pdf (Vector wo, Vector wi)
        {
            if (!Util.SameHemisphere (wo, wi))
                return 0.0;
            return Distribution.Pdf (wo, wi);
        }

        private double G (Vector wo, Vector wi, Vector wh)
        {
            double NdotWh = Math.Abs (Util.CosTheta (wh));
            double NdotWo = Math.Abs (Util.CosTheta (wo));
            double NdotWi = Math.Abs (Util.CosTheta (wi));
            double W0dotWh = Util.AbsDot (wo, wh);
            
            return Math.Min (1.0, Math.Min ((2.0 * NdotWh * NdotWo / W0dotWh), (2.0 * NdotWh * NdotWi / W0dotWh)));
        }
    }
}
