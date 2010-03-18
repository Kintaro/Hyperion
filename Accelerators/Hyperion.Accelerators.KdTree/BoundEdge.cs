
using System;
using System.Collections.Generic;

namespace Hyperion.Accelerators.KdTree
{
    public class BoundEdge : IComparable<BoundEdge>
    {
        public enum EdgeType : byte
        {
            Start = 0,
            End = 1
        }

        public double t;
        public int PrimitiveNumber;
        public EdgeType Type;

        public BoundEdge (double tt, int pn, bool starting)
        {
            this.t = tt;
            this.PrimitiveNumber = pn;
            this.Type = starting ? EdgeType.Start : EdgeType.End;
        }

        public static bool operator < (BoundEdge left, BoundEdge right)
        {
            if (left.t == right.t)
                return left.Type < right.Type;
            else
                return left.t < right.t;
        }

        public static bool operator > (BoundEdge left, BoundEdge right)
        {
            if (left.t == right.t)
                return left.Type > right.Type;
            else
                return left.t > right.t;
        }

        public int CompareTo (BoundEdge other)
        {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            return 0;
        }

    }

    public class BoundEdgeComparer : IComparer<BoundEdge>
    {
        public int Compare (BoundEdge x, BoundEdge y)
        {
            return x.CompareTo (y);
        }

    }
}
