
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class Blinn : IMicrofacetDistribution
    {
        private double Exponent;

        public Blinn (double e)
        {
            if (e > 10000.0 || double.IsNaN (e))
                e = 10000.0;
            Exponent = e;
        }

        public double D (Hyperion.Core.Geometry.Vector wh)
        {
            double cosThetaH = Util.AbsCosTheta (wh);
            return (Exponent + 2) * Util.InvTwoPi * Math.Pow (cosThetaH, Exponent);
        }

        public void SampleF (Vector wo, ref Vector wi, double u1, double u2, ref double pdf)
        {
            double cosTheta = Math.Pow (u1, 1.0 / (Exponent + 1));
            double sinTheta = Math.Sqrt (Math.Max (0.0, 1.0 - cosTheta * cosTheta));
            double phi = u2 * 2.0 * Util.Pi;
            Vector wh = Util.SphericalDirection (sinTheta, cosTheta, phi);
            if (!Util.SameHemisphere (wo, wh))
                wh = -wh;

            wi = -wo + 2.0 * (wo ^ wh) * wh;
            double blinnPdf = ((Exponent + 1.0) * Math.Pow (cosTheta, Exponent)) / (2.0 * Util.Pi * 4.0 * (wo ^ wh));
            if ((wo ^ wh) <= 0.0)
                blinnPdf = 0.0;
            pdf = blinnPdf;
        }

        public double Pdf (Vector wo, Vector wi)
        {
            Vector wh = (wo + wi).Normalized;
            double cosTheta = Util.AbsCosTheta (wh);
            double blinnPdf = ((Exponent + 1.0) * Math.Pow (cosTheta, Exponent)) / (2.0 * Util.Pi * 4.0 * (wo ^ wh));
            if ((wo ^ wh) <= 0.0)
                blinnPdf = 0.0;
            return blinnPdf;
        }
    }
}
