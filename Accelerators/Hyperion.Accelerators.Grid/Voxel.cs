
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;

namespace Hyperion.Accelerators.GridAccelerator
{
    public class Voxel
    {
        private List<IPrimitive> primitives = new List<IPrimitive> ();
        private bool allCanIntersect;

        public Voxel (IPrimitive primitive)
        {
            allCanIntersect = false;
            primitives.Add (primitive);
        }

        public void AddPrimitive (IPrimitive primitive)
        {
            primitives.Add (primitive);
        }

        public bool Intersect (Ray ray, ref Intersection intersection)
        {
            if (!allCanIntersect)
            {
                for (int i = 0; i < primitives.Count; ++i)
                {
                    IPrimitive primitive = primitives[i];
                    if (!primitive.CanIntersect)
                    {
                        List<IPrimitive> p = new List<IPrimitive> ();
                        primitive.FullyRefine (ref p);

                        if (p.Count == 1)
                            primitives[i] = p[0];
                        else
                            primitives[i] = new Grid (p, false);
                    }
                }
                allCanIntersect = true;
            }

            bool hitSomething = false;
            foreach (IPrimitive primitive in primitives)
            {
                if (primitive.Intersect (ray, ref intersection))
                    hitSomething = true;
            }

            return hitSomething;
        }

        public bool IntersectP (Ray ray)
        {
            if (!allCanIntersect)
            {
                for (int i = 0; i < primitives.Count; ++i)
                {
                    IPrimitive primitive = primitives[i];
                    if (!primitive.CanIntersect)
                    {
                        List<IPrimitive> p = new List<IPrimitive> ();
                        primitive.FullyRefine (ref p);
                        
                        if (p.Count == 1)
                            primitives[i] = p[0];
                        else
                            primitives[i] = new Grid (p, false);
                    }
                }
                allCanIntersect = true;
            }

            foreach (IPrimitive primitive in primitives)
            {
                if (primitive.IntersectP (ray))
                    return true;
            }

            return false;
        }
    }
}
