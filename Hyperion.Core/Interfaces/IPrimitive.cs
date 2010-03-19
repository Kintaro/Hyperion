
using System;
using System.Collections.Generic;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;

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

        public virtual void Refine (ref List<IPrimitive> refined)
        {
        }

        public void FullyRefine (ref List<IPrimitive> refined)
        {
            List<IPrimitive> todo = new List<IPrimitive> ();
            todo.Add (this);

            while (todo.Count > 0)
            {
                IPrimitive primitive = todo[todo.Count - 1];
                todo.RemoveAt (todo.Count - 1);
                if (primitive.CanIntersect)
                    refined.Add (primitive);
                else
                    primitive.Refine (ref todo);
            }
        }

        public abstract BoundingBox WorldBound { get; }
        public abstract AreaLight AreaLight { get; }
        public abstract bool Intersect (Ray ray, ref Intersection isect);
        public abstract bool IntersectP (Ray ray);
        public abstract BSDF GetBsdf (DifferentialGeometry dg, Transform objectoToWorld);
        public abstract BSSRDF GetBssrdf (DifferentialGeometry dg, Transform objectToWorld);
    }
}
