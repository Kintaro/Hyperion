
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{


    public class BSDF
    {
        private Normal nn;
        private Normal ng;
        private Vector sn;
        private Vector tn;
        private int nBxDFs;

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
    }
}
