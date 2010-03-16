
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public enum BxDFType
    {
        BSDF_REFLECTION = 1 << 0,
        BSDF_TRANSMISSION = 1 << 1,
        BSDF_DIFFUSE = 1 << 2,
        BSDF_GLOSSY = 1 << 3,
        BSDF_SPECULAR = 1 << 4,
        BSDF_ALL_TYPES = BSDF_DIFFUSE | BSDF_GLOSSY | BSDF_SPECULAR,
        BSDF_ALL_REFLECTION = BSDF_REFLECTION | BSDF_ALL_TYPES,
        BSDF_ALL_TRANSMISSION = BSDF_TRANSMISSION | BSDF_ALL_TYPES,
        BSDF_ALL = BSDF_ALL_REFLECTION | BSDF_ALL_TRANSMISSION
    }

    public abstract class BxDF
    {
        public readonly BxDFType Type;

        public BxDF (BxDFType type)
        {
            this.Type = type;
        }

        public bool MatchesFlags (BxDFType flags)
        {
            return (this.Type & flags) == this.Type;
        }

        public abstract Spectrum F (Vector wo, Vector wi);

        public virtual Spectrum SampleF (Vector wo, ref Vector wi, double u1, double u2, ref double pdf)
        {
            wi = MonteCarlo.CosineSampleHemisphere (u1, u2);
            if (wo.z < 0.0)
                wi.z *= -1.0;
            pdf = Pdf (wo, wi);

            return F (wo, wi);
        }

        public virtual Spectrum Rho (Vector wo, int nSamples, double[] samples)
        {
            Spectrum r = new Spectrum ();
            for (int i = 0; i < nSamples; ++i)
            {
                Vector wi = new Vector ();
                double pdf = 0.0;
                Spectrum f = SampleF (wo, ref wi, samples[2 * i], samples[2 * i + 1], ref pdf);
                if (pdf > 0.0)
                    r += f * Util.AbsCosTheta (wi) / pdf;
            }
            return r / (double)nSamples;
        }

        public virtual Spectrum Rho (int nSamples, double[] samples1, double[] samples2)
        {
            Spectrum r = new Spectrum ();
            for (int i = 0; i < nSamples; ++i)
            {
                Vector wo, wi = new Vector ();
                wo = MonteCarlo.UniformSampleHemisphere (samples1[2 * i], samples1[2 * i + 1]);
                double pdf_o = Util.InvTwoPi, pdf_i = 0.0;
                Spectrum f = SampleF (wo, ref wi, samples2[2 * i], samples2[2 * i + 1], ref pdf_i);

                if (pdf_i > 0.0)
                    r += f * Util.AbsCosTheta (wi) * Util.AbsCosTheta (wo) / (pdf_o * pdf_i);
            }
            return r / (Util.Pi * nSamples);
        }

        public virtual double Pdf (Vector wi, Vector wo)
        {
            return Util.SameHemisphere (wo, wi) ? Util.AbsCosTheta (wi) * Util.InvPi : 0.0;
        }
    }
}
