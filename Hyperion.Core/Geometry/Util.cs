
using System;

namespace Hyperion.Core.Geometry
{
    public static class Util
    {
        public const double Pi = Math.PI;
        public const double InvPi = 1.0 / Math.PI;
        public const double InvTwoPi = 1.0 / (2.0 * Math.PI);
        public const double InvFourPi = 1.0 / (4.0 * Math.PI);
        public static readonly Random Random = new Random ();
        public static readonly double InvLog2 = 1.0 / Math.Log (2.0);

        public static double Distance (Point p1, Point p2)
        {
            return (p1 - p2).Length;
        }

        public static double Lerp (double t, double v1, double v2)
        {
            return (1.0 - t) * v1 + t * v2;
        }

        public static double Clamp (double val, double low, double high)
        {
            if (val < low)
                return low;
            if (val > high)
                return high;
            return val;
        }

        public static int Clamp (int val, int low, int high)
        {
            if (val < low)
                return low;
            if (val > high)
                return high;
            return val;
        }

        public static double DistanceSquared (Point a, Point b)
        {
            return (a - b).SquaredLength;
        }

        public static double AbsDot (Vector a, Vector b)
        {
            return Math.Abs (a ^ b);
        }

        public static double AbsDot (Normal a, Vector b)
        {
            return Math.Abs (a ^ b);
        }

        public static double AbsDot (Vector a, Normal b)
        {
            return Math.Abs (a ^ b);
        }

        public static int Floor2Int (double val)
        {
            return (int)Math.Floor (val);
        }

        public static int Ceil2Int (double val)
        {
            return (int)Math.Ceiling (val);
        }

        public static int Double2Int (double val)
        {
            return (int)val;
        }

        public static double Log2 (double x)
        {
            return Math.Log (x) * InvLog2;
        }

        public static int Log2Int (double v)
        {
            return Floor2Int (Log2 (v));
        }

        public static bool SameHemisphere (Vector w, Vector wp)
        {
            return w.z * wp.z > 0.0;
        }

        public static double AbsCosTheta (Vector w)
        {
            return Math.Abs (w.z);
        }

        public static double Radians (double deg)
        {
            return (Pi / 180.0) * deg;
        }

        public static Normal FaceForward (Normal a, Normal b)
        {
            return ((a ^ b) < 0.0) ? -a : a;
        }

        public static double CosTheta (Vector w)
        {
            return w.z;
        }

        public static double SinTheta (Vector w)
        {
            return Math.Max (0.0, 1.0 - CosTheta (w) * CosTheta (w));
        }

        public static double SinTheta2 (Vector w)
        {
            return Math.Max (0.0, 1.0 - CosTheta (w) * CosTheta (w));
        }

        public static double SinPhi (Vector w)
        {
            double sinTheta = SinTheta (w);
            if (sinTheta == 0.0)
                return 0.0;
            return Clamp (w.y / sinTheta, -1.0, 1.0);
        }

        public static double CosPhi (Vector w)
        {
            double sinTheta = SinTheta (w);
            if (sinTheta == 0.0)
                return 1.0;
            return Clamp (w.x / sinTheta, -1.0, 1.0);
        }

        public static int RoundUpPow2 (int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            return v + 1;
        }

        public static bool IsPowerOf2 (int v)
        {
            return (v & (v - 1)) == 0;
        }

        public static int RoundToInt (double val)
        {
            return Floor2Int (val + 0.5);
        }

        public static void CoordinateSystem (Vector v1, out Vector v2, out Vector v3)
        {
            if (Math.Abs (v1.x) > Math.Abs (v1.y))
            {
                double invLen = 1.0 / Math.Sqrt (v1.x * v1.x + v1.z * v1.z);
                v2 = new Vector (-v1.z * invLen, 0.0, v1.x * invLen);
            } else
            {
                double invLen = 1.0 / Math.Sqrt (v1.y * v1.y + v1.z * v1.z);
                v2 = new Vector (0.0, v1.z * invLen, -v1.y * invLen);
            }
            v3 = (v1 % v2);
        }

        public static bool SolveLinearSystem2x2 (double[][] A, double B0, double B1, out double x0, out double x1)
        {
            x0 = 0.0;
            x1 = 0.0;
            double det = A[0][0] * A[1][1] - A[0][1] * A[1][0];
            if (Math.Abs (det) < 1E-10)
                return false;
            x0 = (A[1][1] * B0 - A[0][1] * B1) / det;
            x1 = (A[0][0] * B1 - A[1][0] * B0) / det;
            if (double.IsNaN (x0) || double.IsNaN (x1))
                return false;
            return true;
        }

        public static bool Quadratic (double A, double B, double C, ref double t0, ref double t1)
        {
            // Find quadratic discriminant
            double discrim = B * B - 4.0 * A * C;
            if (discrim <= 0.0)
                return false;
            double rootDiscrim = Math.Sqrt (discrim);
            
            // Compute quadratic _t_ values
            double q;
            if (B < 0)
                q = -0.5 * (B - rootDiscrim);
            else
                q = -0.5 * (B + rootDiscrim);
            t0 = q / A;
            t1 = C / q;
            if (t0 > t1)
            {
                double t = t0;
                t0 = t1;
                t1 = t;
            }
            return true;
        }
    }
}
