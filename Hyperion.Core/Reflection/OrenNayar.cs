
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class OrenNayar : BxDF
    {
        private Spectrum R;
        private double A;
        private double B;

        public OrenNayar (Spectrum reflectance, double sig) : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE)
        {
            double sigma = Util.Radians (sig);
            double sigma2 = sigma * sigma;
            A = 1.0 - (sigma2 / (2.0 * (sigma2 + 0.33)));
            B = 0.45 * sigma2 / (sigma2 + 0.09);
        }

        public override Spectrum F (Vector wo, Vector wi)
        {
            double sinThetaI = Util.SinTheta (wi);
            double sinThetaO = Util.SinTheta (wo);
            double maxCos = 0.0;

            if (sinThetaI > 1e-4 && sinThetaO > 1e-4)
            {
                double sinPhiI = Util.SinPhi (wi), cosPhiI = Util.CosPhi (wi);
                double sinPhiO = Util.SinPhi (wo), cosPhiO = Util.CosPhi (wo);
                double dcos = cosPhiI * cosPhiO + sinPhiI * sinPhiO;
                maxCos = Math.Max (0.0, dcos);
            }

            double sinAlpha, tanBeta;
            if (Util.AbsCosTheta (wi) > Util.AbsCosTheta (wo))
            {
                sinAlpha = sinThetaO;
                tanBeta = sinThetaI / Util.AbsCosTheta (wi);
            }
            else
            {
                sinAlpha = sinThetaI;
                tanBeta = sinThetaO / Util.AbsCosTheta (wo);
            }

            return R * Util.InvPi * (A + B * maxCos * sinAlpha * tanBeta);
        }
    }
}
