
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
    }
}
