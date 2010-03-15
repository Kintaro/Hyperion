
using System;

namespace Hyperion.Core.Geometry
{
    public class Ray
    {
        public Point Origin;
        public Vector Direction;
        public double MinT;
        public double MaxT;
        public double Time;
        public int Depth;

        public Ray ()
        {
            MinT = 0.0;
            MaxT = double.PositiveInfinity;
            Time = 0.0;
            Depth = 0;
            Origin = new Point ();
            Direction = new Vector ();
        }

        public Ray (Point origin, Vector direction, double start) : this(origin, direction, start, double.PositiveInfinity, 0.0, 0)
        {
        }

        public Ray (Point origin, Vector direction, double start, double end) : this(origin, direction, start, end, 0.0, 0)
        {
        }

        public Ray (Point origin, Vector direction, double start, double end, double time) : this(origin, direction, start, end, time, 0)
        {
        }

        public Ray (Point origin, Vector direction, double start, double end, double time, int depth)
        {
            Origin = origin;
            Direction = direction;
            MinT = start;
            MaxT = end;
            Time = time;
            Depth = depth;
        }

        public Ray (Point origin, Vector direction, Ray parent, double start, double end) : this(origin, direction, start, end, parent.Time, parent.Depth + 1)
        {
        }

        public Point Apply (double time)
        {
            return Origin + Direction * time;
        }

        public bool HasNaNs
        {
            get
            {
                return Origin.HasNaNs || Direction.HasNaNs || double.IsNaN (MinT) || double.IsNaN (MaxT);
            }
        }
    }
}
