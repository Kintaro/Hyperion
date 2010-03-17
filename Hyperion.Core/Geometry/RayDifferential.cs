
using System;

namespace Hyperion.Core.Geometry
{
    public sealed class RayDifferential : Ray
    {
        public bool HasDifferentials;
        public Point RxOrigin;
        public Point RyOrigin;
        public Vector RxDirection;
        public Vector RyDirection;

        public RayDifferential ()
        {
            RxOrigin = new Point ();
            RyOrigin = new Point ();
            RxDirection = new Vector ();
            RyDirection = new Vector ();
            HasDifferentials = false;
        }

        public RayDifferential (Point origin, Vector direction, double start, double end) : this(origin, direction, start, end, 0.0, 0)
        {
            
        }

        public RayDifferential (Point origin, Vector direction, double start, double end, double time, int depth) : base(origin, direction, start, end, time, depth)
        {
            HasDifferentials = false;
        }

        public RayDifferential (Ray ray) : this(ray.Origin, ray.Direction, ray.MinT, ray.MaxT, ray.Time, ray.Depth)
        {
        }

        public RayDifferential (Point origin, Vector direction, Ray parent, double start) : this(origin, direction, parent, start, double.PositiveInfinity)
        {
        }

        public RayDifferential (Point origin, Vector direction, Ray parent, double start, double end) : base(origin, direction, start, end, parent.Time, parent.Depth + 1)
        {
            HasDifferentials = false;
        }

        public void ScaleDifferentials (double s)
        {
            RxOrigin = Origin + (RxOrigin - Origin) * s;
            RyOrigin = Origin + (RyOrigin - Origin) * s;
            RxDirection = Direction + (RxDirection - Direction) * s;
            RyDirection = Direction + (RyDirection - Direction) * s;
        }
    }
}
