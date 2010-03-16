
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public class Intersection
    {
        public IPrimitive Primitive;
        public Transform WorldToObject;
        public Transform ObjectToWorld;
        public int ShapeID;
        public int PrimitiveID;
        public double RayEpsilon;
        public DifferentialGeometry dg;

        public Intersection ()
        {
        }
    }
}
