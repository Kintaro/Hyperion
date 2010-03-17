
using System;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;

namespace Hyperion.Core.Interfaces
{
    public sealed class Intersection
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

        public BSDF GetBSDF (RayDifferential ray)
        {
            dg.ComputeDifferentials (ray);
            return Primitive.GetBsdf (dg, ObjectToWorld);
        }

        public Spectrum Le (Vector wo)
        {
            AreaLight area = Primitive.AreaLight;
            return area != null ? area.L (dg.p, dg.n, wo) : new Spectrum ();
        }
    }
}
