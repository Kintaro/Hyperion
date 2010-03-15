
using System;

namespace Hyperion.Core.Geometry
{
    public class BoundingBox
    {
        public Point pMin;
        public Point pMax;

        public BoundingBox ()
        {
            pMin = new Point (double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            pMax = new Point (-double.PositiveInfinity, -double.PositiveInfinity, -double.PositiveInfinity);
        }

        public BoundingBox (Point p)
        {
            pMin = new Point (p);
            pMax = new Point (p);
        }

        public BoundingBox (Point p1, Point p2)
        {
            pMin = new Point (p1);
            pMax = new Point (p2);
        }
    }
}
