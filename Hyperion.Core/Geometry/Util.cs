
using System;

namespace Hyperion.Core.Geometry
{
    public static class Util
    {
        public static double Distance (Point p1, Point p2)
        {
            return (p1 - p2).Length;
        }
    }
}
