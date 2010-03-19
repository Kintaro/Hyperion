
using System;

namespace Hyperion.Core.Geometry
{
    public sealed class BoundingBox
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

        public BoundingBox (BoundingBox b)
        {
            pMin = new Point (b.pMin);
            pMax = new Point (b.pMax);
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

        public void BoundingSphere (out Point center, out double radius)
        {
            center = 0.5 * pMin + 0.5 * pMax;
            radius = Inside (center) ? Util.Distance (center, pMax) : 0.0;
        }

        public bool IntersectP (Ray ray, out double hit0)
        {
            double temp = 0.0;
            return IntersectP (ray, out hit0, out temp);
        }

        public bool IntersectP (Ray ray, out double hit0, out double hit1)
        {
            double t0 = ray.MinT, t1 = ray.MaxT;

            for (int i = 0; i < 3; ++i)
            {
                double invRayDir = 1.0 / ray.Direction[i];
                double near = (pMin[i] - ray.Origin[i]) * invRayDir;
                double far = (pMax[i] - ray.Origin[i]) * invRayDir;

                if (near > far)
                {
                    double temp = near;
                    near = far;
                    far = temp;
                }

                t0 = near > t0 ? near : t0;
                t1 = far < t1 ? far : t1;

                hit0 = t0;
                hit1 = t1;

                if (t0 > t1)
                    return false;
            }

            hit0 = t0;
            hit1 = t1;
            return true;
        }

        public Point Lerp (double tx, double ty, double tz)
        {
            return new Point (Util.Lerp (tx, pMin.x, pMax.x),
                Util.Lerp (ty, pMin.y, pMax.y),
                Util.Lerp (tz, pMin.z, pMax.z));
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

        public static BoundingBox Union (BoundingBox b, Point p)
        {
            BoundingBox ret = new BoundingBox (b);
            ret.pMin.x = Math.Min (b.pMin.x, p.x);
            ret.pMin.y = Math.Min (b.pMin.y, p.y);
            ret.pMin.z = Math.Min (b.pMin.z, p.z);
            ret.pMax.x = Math.Max (b.pMax.x, p.x);
            ret.pMax.y = Math.Max (b.pMax.y, p.y);
            ret.pMax.z = Math.Max (b.pMax.z, p.z);
            return ret;
        }

        public static BoundingBox Union (BoundingBox b, BoundingBox b2)
        {
            BoundingBox ret = new BoundingBox (b);
            ret.pMin.x = Math.Min (b.pMin.x, b2.pMin.x);
            ret.pMin.y = Math.Min (b.pMin.y, b2.pMin.y);
            ret.pMin.z = Math.Min (b.pMin.z, b2.pMin.z);
            ret.pMax.x = Math.Max (b.pMax.x, b2.pMax.x);
            ret.pMax.y = Math.Max (b.pMax.y, b2.pMax.y);
            ret.pMax.z = Math.Max (b.pMax.z, b2.pMax.z);
            return ret;
        }

        public override string ToString ()
        {
            return string.Format ("[pMin: {0}, pMax: {1}]", pMin, pMax);
        }
    }
}
