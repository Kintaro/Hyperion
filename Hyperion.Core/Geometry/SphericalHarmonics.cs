
using System;

namespace Hyperion.Core.Geometry
{
    public static class SphericalHarmonics
    {
        public static int SHTerms (int lmax)
        {
            return (lmax + 1) * (lmax + 1);
        }

        public static int SHIndex (int l, int m)
        {
            return l * l + l + m;
        }

        public static double K (int l, int m)
        {
            return Math.Sqrt ((2.0 * l + 1.0) * Util.InvFourPi * DivFact (l, m));
        }

        public static double DivFact (int a, int b)
        {
            if (b == 0)
                return 1.0;
            double fa = a, fb = Math.Abs (b);
            double v = 1.0;
            for (double x = fa - fb + 1.0; x <= fa + fb; x += 1.0)
                v *= x;
            return 1.0 / v;
        }

        public static void LegendRep (double x, int lmax, double[] o)
        {
            o[SHIndex (0, 0)] = 1.0;
            o[SHIndex (1, 0)] = x;

            for (int l = 2; l <= lmax; ++l)
            {
                o[SHIndex (l, 0)] = ((2 * l - 1) * x * o[SHIndex (l - 1, 0)] - (l - 1) * o[SHIndex (l - 2, 0)]) / l;
            }

            double neg = -1.0;
            double dfact = 1.0;
            double xroot = Math.Sqrt (Math.Max (0.0, 1.0 - x * x));
            double xpow = xroot;

            for (int l = 1; l <= lmax; ++l)
            {
                o[SHIndex (l, l)] = neg * dfact * xpow;
                neg *= -1.0;
                dfact *= 2 * l + 1;
                xpow *= xroot;
            }

            for (int l = 2; l<= lmax; ++l)
            {
                o[SHIndex (l, l - 1)] = x * (2 * l - 1) * o[SHIndex (l - 1, l - 1)];
            }

            for (int l = 3; l <= lmax; ++l)
            {
                for (int m = 1; m <= l - 2; ++m)
                {
                    o[SHIndex (l, m)] = ((2 * (l - 1) + 1) * x * o[SHIndex (l - 1, m)] - (l - 1 + m) * o[SHIndex (l - 2, m)]) / (l - m);
                }
            }
        }

        public static void SinCosIndexed (double s, double c, int n, double[] sout, double[] cout)
        {
            int sindex = 0, cindex = 0;
            double si = 0, ci = 1;
            for (int i = 0; i < n; ++i)
            {

                sout[sindex++] = si;
                cout[cindex++] = ci;
                double oldsi = si;
                si = si * c + ci * s;
                ci = ci * c - oldsi * s;
            }
        }

        public static void SHEvaluate (Vector w, int lmax, double[] o)
        {
            if (lmax > 28)
                ;
            LegendRep (w.z, lmax, o);

            double[] Klm = new double[SHTerms (lmax)];
            for (int l = 0; l <= lmax; ++l)
                for (int m = -l; m <= l; ++m)
                    Klm[SHIndex (l, m)] = K (l, m);

            double[] sins = new double[lmax + 1], coss = new double[lmax + 1];
            double xyLength = Math.Sqrt (Math.Max (0.0, 1.0 - w.z * w.z));

            if (xyLength == 0.0)
            {
                for (int i = 0; i <= lmax; ++i)
                    sins[i] = 0.0;
                for (int i = 0; i <= lmax; ++i)
                    coss[i] = 1.0;
            }
            else
                SinCosIndexed (w.y / xyLength, w.x / xyLength, lmax + 1, sins, coss);

            double sqrt2 = Math.Sqrt (2.0);

            for (int l = 0; l <= lmax; ++l)
            {
                for (int m = -l; m < 0; ++m)
                {
                    o[SHIndex (l, m)] = sqrt2 * Klm[SHIndex (l, m)] * o[SHIndex (l, -m)] * sins[-m];
                }
                o[SHIndex (l, 0)] *= Klm[SHIndex (l, 0)];
                for (int m = 1; m <= l; ++m)
                {
                    o[SHIndex (l, m)] *= sqrt2 * Klm[SHIndex (l, m)] * coss[m];
                }
            }
        }
    }
}
