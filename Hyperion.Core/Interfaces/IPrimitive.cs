
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public abstract class IPrimitive
    {
        private static int NextPrimitiveID;
        public readonly int PrimitiveID;

        public IPrimitive ()
        {
            PrimitiveID = NextPrimitiveID++;
        }

        public virtual bool CanIntersect {
            get { return true; }
        }

        public abstract BoundingBox WorldBound { get; }

        public abstract bool Intersect (Ray ray, ref Intersection isect);
        public abstract bool IntersectP (Ray ray);
    }
}
