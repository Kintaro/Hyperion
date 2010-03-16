
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
    }
}
