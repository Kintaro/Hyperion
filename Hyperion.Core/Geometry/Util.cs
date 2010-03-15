
using System;

namespace Hyperion.Core.Geometry
{
    public static class Util
    {
        public static double Distance (Point p1, Point p2)
        {
            return (p1 - p2).Length;
        }

        public static double Lerp (double t, double v1, double v2)
        {
            return (1.0 - t) * v1 + t * v2;
        }
    }
}
