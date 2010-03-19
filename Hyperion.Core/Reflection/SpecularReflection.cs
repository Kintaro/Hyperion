
using System;
using Hyperion.Core;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class SpecularReflection : BxDF
    {
        private Spectrum R;
        private IFresnel Fresnel;

        public SpecularReflection (Spectrum r, IFresnel f) : base (BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR)
        {
            R = r;
            Fresnel = f;
        }

        public override Spectrum F (Vector wo, Vector wi)
        {
            return new Spectrum ();
        }

        public override Spectrum SampleF (Vector wo, ref Vector wi, double u1, double u2, ref double pdf)
        {
            wi = new Vector (-wo.x, -wo.y, wo.z);
            pdf = 1.0;
            return Fresnel.Evaluate (Util.CosTheta (wo)) * R / Util.AbsCosTheta (wi);
        }

        public override double Pdf (Vector wi, Vector wo)
        {
            return 0.0;
        }
    }
}
