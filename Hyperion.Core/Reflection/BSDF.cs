
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public sealed class BSDF
    {
        private Normal nn;
        private Normal ng;
        private Vector sn;
        private Vector tn;
        private int nBxDFs;
        private BxDF[] bxdfs = new BxDF[8];

        public readonly DifferentialGeometry dgShading;
        public readonly double Eta;

        public BSDF (DifferentialGeometry dgs, Normal ngeom) : this(dgs, ngeom, 1.0)
        {
        }

        public BSDF (DifferentialGeometry dgs, Normal ngeom, double eta)
        {
            dgShading = new DifferentialGeometry (dgs);
            Eta = eta;
            ng = new Normal (ngeom);
            nn = new Normal (dgShading.n);
            sn = dgShading.dpdu.Normalized;
            tn = nn % sn;
            nBxDFs = 0;
        }

        public double Pdf (Vector wo, Vector wi)
        {
            return Pdf (wo, wi, BxDFType.BSDF_ALL);
        }

        public double Pdf (Vector woW, Vector wiW, BxDFType flags)
        {
            if (nBxDFs == 0)
                return 0.0;
            Vector wo = WorldToLocal (woW), wi = WorldToLocal (wiW);
            double pdf = 0.0;
            int matchingComps = 0;
            for (int i = 0; i < nBxDFs; ++i)
            {
                if (bxdfs[i].MatchesFlags (flags))
                {
                    ++matchingComps;
                    pdf += bxdfs[i].Pdf (wo, wi);
                }
            }
            return matchingComps > 0 ? pdf / matchingComps : 0.0;
        }

        public Vector WorldToLocal (Vector v)
        {
            return new Vector (v ^ sn, v ^ tn, v ^ nn);
        }

        public Vector LocalToWorld (Vector v)
        {
            return new Vector (sn.x * v.x + tn.x * v.y + nn.x * v.z, sn.y * v.x + tn.y * v.y + nn.y * v.z, sn.z * v.x + tn.z * v.y + nn.z * v.z);
        }

        public int NumComponents ()
        {
            return nBxDFs;
        }

        public int NumComponents (BxDFType flags)
        {
            int num = 0;
            for (int i = 0; i < nBxDFs; ++i)
                if (bxdfs[i].MatchesFlags (flags))
                    ++num;
            return num;
        }

        public void Add (BxDF bxdf)
        {
            bxdfs[nBxDFs++] = bxdf;
        }

        public Spectrum F (Vector woW, Vector wiW)
        {
            return F (woW, wiW, BxDFType.BSDF_ALL);
        }

        public Spectrum F (Vector woW, Vector wiW, BxDFType flags)
        {
            Vector wi = WorldToLocal (wiW), wo = WorldToLocal (woW);
            if ((wiW ^ ng) * (woW ^ ng) > 0)
                flags &= ~BxDFType.BSDF_TRANSMISSION;
            else
                flags &= ~BxDFType.BSDF_REFLECTION;
            Spectrum f = new Spectrum ();
            for (int i = 0; i < nBxDFs; ++i)
                if (bxdfs[i].MatchesFlags (flags))
                    f += bxdfs[i].F (wo, wi);
            return f;
        }

        public Spectrum Rho ()
        {
            return Rho (BxDFType.BSDF_ALL, 6);
        }

        public Spectrum Rho (BxDFType flags)
        {
            return Rho (flags, 6);
        }

        public Spectrum Rho (BxDFType flags, int sqrtSamples)
        {
            int nSamples = sqrtSamples * sqrtSamples;
            double[] s1 = new double[2 * nSamples];
            MonteCarlo.StratifiedSample2D (s1, sqrtSamples, sqrtSamples, true);
            double[] s2 = new double[2 * nSamples];
            MonteCarlo.StratifiedSample2D (s2, sqrtSamples, sqrtSamples, true);

            Spectrum ret = new Spectrum ();
            for (int i = 0; i < nBxDFs; ++i)
                if (bxdfs[i].MatchesFlags (flags))
                    ret += bxdfs[i].Rho (nSamples, s1, s2);
            return ret;
        }

        public Spectrum Rho (Vector wo)
        {
            return Rho (wo, BxDFType.BSDF_ALL, 6);
        }

        public Spectrum Rho (Vector wo, BxDFType flags)
        {
            return Rho (wo, flags, 6);
        }

        public Spectrum Rho (Vector wo, BxDFType flags, int sqrtSamples)
        {
            int nSamples = sqrtSamples * sqrtSamples;
            double[] s1 = new double[2 * nSamples];
            MonteCarlo.StratifiedSample2D (s1, sqrtSamples, sqrtSamples, true);
            Spectrum ret = new Spectrum ();
            for (int i = 0; i < nBxDFs; ++i)
                if (bxdfs[i].MatchesFlags (flags))
                    ret += bxdfs[i].Rho (wo, nSamples, s1);
            return ret;
        }

        public Spectrum SampleF (Vector wo, ref Vector wi, BSDFSample bsdfSample, ref double pdf)
        {
            BxDFType temp = BxDFType.BSDF_ALL;
            return SampleF (wo, ref wi, bsdfSample, ref pdf, BxDFType.BSDF_ALL, ref temp);
        }

        public Spectrum SampleF (Vector wo, ref Vector wi, BSDFSample bsdfSample, ref double pdf, BxDFType flags)
        {
            BxDFType temp = BxDFType.BSDF_ALL;
            return SampleF (wo, ref wi, bsdfSample, ref pdf, flags, ref temp);
        }

        public Spectrum SampleF (Vector woW, ref Vector wiW, BSDFSample bsdfSample, ref double pdf, BxDFType flags, ref BxDFType sampledType)
        {
            int matchingComps = NumComponents (flags);
            if (matchingComps == 0)
            {
                pdf = 0.0;
                return new Spectrum ();
            }

            int which = Math.Min (Util.Floor2Int (bsdfSample.uComponent * matchingComps), matchingComps - 1);

            BxDF bxdf = null;
            int count = which;
            for (int i = 0; i < nBxDFs; ++i)
            {
                if (bxdfs[i].MatchesFlags (flags) && count-- == 0)
                {
                    bxdf = bxdfs[i];
                    break;
                }
            }

            Vector wo = WorldToLocal (woW);
            Vector wi = new Vector ();
            pdf = 0.0;
            Spectrum f = bxdf.SampleF (wo, ref wi, bsdfSample.uDir[0], bsdfSample.uDir[1], ref pdf);

            if (pdf == 0.0)
            {
                return new Spectrum ();
            }

            sampledType = bxdf.Type;
            wiW = LocalToWorld (wi);

            if ((bxdf.Type & BxDFType.BSDF_SPECULAR) == 0 && matchingComps > 1)
                for (int i = 0; i < nBxDFs; ++i)
                    if (bxdfs[i] != bxdf && bxdfs[i].MatchesFlags (flags))
                        pdf += bxdfs[i].Pdf (wo, wi);
            if (matchingComps > 1)
                pdf /= matchingComps;

            if ((bxdf.Type & BxDFType.BSDF_SPECULAR) == 0)
            {
                f = new Spectrum ();
                if ((wiW ^ ng) * (woW ^ ng) > 0.0)
                    flags &= ~BxDFType.BSDF_TRANSMISSION;
                else
                    flags &= ~BxDFType.BSDF_REFLECTION;

                for (int i = 0; i < nBxDFs; ++i)
                    if (bxdfs[i].MatchesFlags (flags))
                        f += bxdfs[i].F (wo, wi);
            }

            return f;
        }
    }
}
