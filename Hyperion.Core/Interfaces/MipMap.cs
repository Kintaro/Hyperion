
using System;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Interfaces
{
    public class MipMap<T> where T : TexelConstraint<T>, new ()
    {
        private const int WeightLutSize = 128;
        private static double[] WeightLut;
        private BlockedArray<T>[] Pyramid;
        private int Width;
        private int Height;
        private int NumberOfLevels;
        private bool DoTrilinear;
        private double MaxAnisotropy;

        public MipMap ()
        {
        }

        public T Texel (int level, int s, int t)
        {
            BlockedArray<T> l = Pyramid[level];
            return l.Get (s, t);
        }

        private T Triangle (int level, double s, double t)
        {
            level = Util.Clamp (level, 0, NumberOfLevels - 1);
            s = s * Pyramid[level].uSize - 0.5;
            t = t * Pyramid[level].vSize - 0.5;
            int s0 = Util.Floor2Int (s), t0 = Util.Floor2Int (t);
            double ds = s - s0, dt = t - t0;

            return Texel (level, s0, t0).Mul ((1.0 - ds) * (1.0 - dt)).Add (Texel (level, s0, t0 + 1).Mul ((1.0 - ds) * dt)).Add (Texel (level, s0 + 1, t0).Mul (ds * (1.0 - dt))).Add (Texel (level, s0 + 1, t0 + 1).Mul (ds * dt));
        }

        private T EWA (int level, double s, double t, double ds0, double dt0, double ds1, double dt1)
        {
            if (level >= NumberOfLevels)
                return Texel (NumberOfLevels - 1, 0, 0);
            s = s * Pyramid[level].uSize - 0.5;
            t = t * Pyramid[level].vSize - 0.5;
            ds0 *= Pyramid[level].uSize;
            dt0 *= Pyramid[level].vSize;
            ds1 *= Pyramid[level].uSize;
            dt1 *= Pyramid[level].vSize;

            double A = dt0 * dt0 + dt1 * dt1 + 1;
            double B = -2.5 * (ds0 * dt0 + ds1 * dt1);
            double C = ds0 * ds0 + ds1 * ds1 + 1;
            double invf = 1.0 / (A * C - B * B * 0.25);
            A *= invf;
            B *= invf;
            C *= invf;

            double det = -B*B+4.0*A*C;
            double invDet = 1.0 / det;
            double uSqrt = Math.Sqrt (det * C), vSqrt = Math.Sqrt (A * det);
            int s0 = Util.Ceil2Int (s - 2.0 * invDet * uSqrt);
            int s1 = Util.Floor2Int (s + 2.0 * invDet * uSqrt);
            int t0 = Util.Ceil2Int (t - 2.0 * invDet * vSqrt);
            int t1 = Util.Floor2Int (t + 2.0 * invDet * vSqrt);

            T sum = default(T);
            double sumWts = 0.0;
            for (int it = t0; it <= t1; ++it)
            {
                double tt = it - t;
                for (int si = s0; si <= s1; ++si)
                {
                    double ss = si - s;
                    double r2 = A * ss * ss + B * ss * tt + C * tt * tt;
                    if (r2 < 1.0)
                    {
                        double weight = WeightLut[Math.Min (Util.Double2Int (r2 * WeightLutSize), WeightLutSize - 1)];
                        sum.Add (Texel (level, si, it).Mul (weight));
                        sumWts += weight;
                    }
                }
            }

            return sum.Div (sumWts);
        }

        private double Clamp (double v)
        {
            return Util.Clamp (v, 0.0, double.PositiveInfinity);
        }

        private Spectrum Clamp (Spectrum v)
        {
            return v.Clamp (0.0, double.PositiveInfinity);
        }
    }
}
