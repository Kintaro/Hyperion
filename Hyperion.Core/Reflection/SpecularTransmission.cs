
using System;
using Hyperion.Core;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class SpecularTransmission : BxDF
    {
        private Spectrum T;
        private double EtaI;
        private double EtaT;
        private FresnelDielectric Fresnel;

        public SpecularTransmission (Spectrum t, double ei, double et) : base(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR)
        {
            T = t;
            EtaI = ei;
            EtaT = et;
            Fresnel = new FresnelDielectric (ei, et);
        }

        public override Spectrum F (Vector wo, Vector wi)
        {
            return new Spectrum ();
        }

        public override Spectrum SampleF (Vector wo, ref Vector wi, double u1, double u2, ref double pdf)
        {
            bool entering = Util.CosTheta (wo) > 0.0;
            double ei = EtaI, et = EtaT;
            if (!entering)
            {
                double t = ei;
                ei = et;
                et = t;
            }
            
            // Compute transmitted ray direction
            double sini2 = Util.SinTheta2 (wo);
            double eta = ei / et;
            double sint2 = eta * eta * sini2;
            
            // Handle total internal reflection for transmission
            if (sint2 >= 1.0)
                return new Spectrum ();
            double cost = Math.Sqrt (Math.Max (0.0, 1.0 - sint2));
            if (entering)
                cost = -cost;
            double sintOverSini = eta;
            wi = new Vector (sintOverSini * -wo.x, sintOverSini * -wo.y, cost);
            pdf = 1.0;
            Spectrum F = Fresnel.Evaluate (Util.CosTheta (wo));
            return             /* (et*et)/(ei*ei) * */(new Spectrum (1.0) - F) * T / Util.AbsCosTheta (wi);
        }


        public override double Pdf (Vector wi, Vector wo)
        {
            return 0.0;
        }
    }
}
