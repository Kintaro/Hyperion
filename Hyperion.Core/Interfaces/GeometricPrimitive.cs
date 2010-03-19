
using System;
using System.Collections.Generic;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;

namespace Hyperion.Core.Interfaces
{
    public class GeometricPrimitive : IPrimitive
    {
        private IShape Shape;
        private IMaterial Material;
        private AreaLight areaLight;

        public GeometricPrimitive (IShape shape, IMaterial material, AreaLight areaLight)
        {
            this.Shape = shape;
            this.Material = material;
            this.areaLight = areaLight;
        }

        public override bool CanIntersect {
            get {
                return Shape.CanIntersect;
            }
        }

        public override BoundingBox WorldBound {
            get {
                return Shape.WorldBound;
            }
        }

        public override AreaLight AreaLight
        {
            get
            {
                return areaLight;
            }
        }

        public override bool IntersectP (Ray ray)
        {
            return Shape.IntersectP (ray);
        }

        public override bool Intersect (Ray ray, ref Intersection isect)
        {
            double thit = 0.0, rayEpsilon = 0.0;
            if (!Shape.Intersect (ray, ref thit, ref rayEpsilon, ref isect.dg))
                return false;

            isect.Primitive = this;
            isect.WorldToObject = Shape.WorldToObject;
            isect.ObjectToWorld = Shape.ObjectToWorld;
            isect.ShapeID = Shape.ShapeID;
            isect.PrimitiveID = PrimitiveID;
            isect.RayEpsilon = rayEpsilon;
            ray.MaxT = thit;

            return true;
        }

        public override BSDF GetBsdf (DifferentialGeometry dg, Transform objectoToWorld)
        {
            DifferentialGeometry dgs;
            Shape.GetShadingGeometry (objectoToWorld, dg, out dgs);
            return Material.GetBsdf (dg, dgs);
        }

        public override BSSRDF GetBssrdf (DifferentialGeometry dg, Transform objectToWorld)
        {
            DifferentialGeometry dgs;
            Shape.GetShadingGeometry (objectToWorld, dg, out dgs);
            return Material.GetBssrdf (dg, dgs);
        }

        public override void Refine (ref List<IPrimitive> refined)
        {
            List<IShape> r = new List<IShape> ();
            Shape.Refine (ref r);
            foreach (IShape shape in r)
                refined.Add (new GeometricPrimitive (shape, Material, AreaLight));
        }
    }
}
