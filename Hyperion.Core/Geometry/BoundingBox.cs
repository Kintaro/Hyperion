
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
            pMin = new Point (Math.Min (p1.x, p2.x), Math.Min (p1.y, p2.y), Math.Min (p1.z, p2.z));
            pMax = new Point (Math.Max (p1.x, p2.x), Math.Max (p1.y, p2.y), Math.Max (p1.z, p2.z));
        }

        public bool Overlaps (BoundingBox b)
        {
            bool x = (pMax.x >= b.pMin.x) && (pMin.x <= b.pMax.x);
            bool y = (pMax.y >= b.pMin.y) && (pMin.y <= b.pMax.y);
            bool z = (pMax.z >= b.pMin.z) && (pMin.z <= b.pMax.z);
            return (x && y && z);
        }

        public bool Inside (Point pt)
        {
            return (pt.x >= pMin.x && pt.x <= pMax.x && pt.y >= pMin.y && pt.y <= pMax.y && pt.z >= pMin.z && pt.z <= pMax.z);
        }

        public void Expand (double delta)
        {
            pMin -= new Vector (delta, delta, delta);
            pMax += new Vector (delta, delta, delta);
        }

        public Vector Offset (Point p)
        {
            return new Vector ((p.x - pMin.x) / (pMax.x - pMin.x), (p.y - pMin.y) / (pMax.y - pMin.y), (p.z - pMin.z) / (pMax.z - pMin.z));
        }

        public double SurfaceArea
        {
            get
            {
                Vector d = pMax - pMin;
                return 2.0 * (d.x * d.y + d.x * d.z + d.y * d.z);
            }
        }

        public double Volume
        {
            get
            {
                Vector d = pMax - pMin;
                return d.x * d.y * d.z;
            }
        }

        public int MaximumExtent
        {
            get
            {
                Vector diag = pMax - pMin;
                if (diag.x > diag.y && diag.x > diag.z)
                    return 0;
                else if (diag.y > diag.z)
                    return 1;
                else
                    return 2;
            }
        }
    }
}
