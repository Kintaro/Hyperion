
using System;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;

namespace Hyperion.Core.Interfaces
{
    public abstract class IMaterial
    {
        public virtual BSSRDF GetBssrdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading)
        {
            return null;
        }

        public abstract BSDF GetBsdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading);

        public static void Bump (ITexture<double> d, DifferentialGeometry dgGeom, DifferentialGeometry dgShading, ref DifferentialGeometry dgBump)
        {
            DifferentialGeometry dgEval = new DifferentialGeometry (dgShading);

            double du = 0.5 * (Math.Abs (dgShading.dudx) + Math.Abs (dgShading.dudy));
            if (du == 0.0)
                du = 0.01;
            dgEval.p = dgShading.p + du * dgShading.dpdu;
            dgEval.u = dgShading.u + du;
            dgEval.n = (new Normal(dgShading.dpdu % dgShading.dpdv) + du * dgShading.dndu).Normalized;

            double uDisplace = d.Evaluate (dgEval);

            double dv = 0.5 * (Math.Abs (dgShading.dvdx) + Math.Abs (dgShading.dvdy));
            if (dv == 0.0)
                dv = 0.01;
            dgEval.p = dgShading.p + dv * dgShading.dpdv;
            dgEval.u = dgShading.u;
            dgEval.v = dgShading.v + dv;
            dgEval.n = (new Normal(dgShading.dpdu % dgShading.dpdv) + dv * dgShading.dndv).Normalized;

            double vDisplace = d.Evaluate (dgEval);
            double displace = d.Evaluate (dgShading);

            dgBump = new DifferentialGeometry (dgShading);
            dgBump.dpdu = dgShading.dpdu + (uDisplace - displace) / du * new Vector (dgShading.n) + displace * new Vector (dgShading.dndu);
            dgBump.dpdv = dgShading.dpdv + (vDisplace - displace) / dv * new Vector (dgShading.n) + displace * new Vector (dgShading.dndv);
            dgBump.n = new Normal((dgBump.dpdu % dgBump.dpdv).Normalized);

            if (dgShading.Shape.ReverseOrientation ^ dgShading.Shape.TransformSwapsHandedness)
                dgBump.n *= -1.0;

            dgBump.n = Util.FaceForward (dgBump.n, dgGeom.n);
        }
    }
}
