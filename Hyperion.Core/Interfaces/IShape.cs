
using System;
using System.Collections.Generic;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public abstract class IShape
    {
        public static int NextShapeID;
        public readonly Transform ObjectToWorld;
        public readonly Transform WorldToObject;
        public readonly bool ReverseOrientation;
        public readonly bool TransformSwapsHandedness;
        public readonly int ShapeID;

        public IShape (Transform objectToWorld, Transform worldToObject, bool reverseOrientation)
        {
            ObjectToWorld = new Transform (objectToWorld);
            WorldToObject = new Transform (worldToObject);
            ReverseOrientation = reverseOrientation;
            TransformSwapsHandedness = ObjectToWorld.SwapsHandedness;
            ShapeID = NextShapeID++;
        }

        public abstract BoundingBox ObjectBound
        {
            get;
        }

        public virtual BoundingBox WorldBound
        {
            get
            {
                return ObjectToWorld.Apply (ObjectBound);
            }
        }

        public virtual bool CanIntersect
        {
            get
            {
                return true;
            }
        }

        public virtual double Area
        {
            get
            {
                return 0.0;
            }
        }

        public virtual double Pdf (Point pshape)
        {
            return 1.0 / Area;
        }

        public virtual double Pdf (Point p, Vector wi)
        {
            DifferentialGeometry dgLight = new DifferentialGeometry ();
            Ray ray = new Ray (p, wi, 1e-3);
            ray.Depth = - 1;
            double thit = 0.0, rayEpsilon = 0.0;

            if (!Intersect (ray, ref thit, ref rayEpsilon, ref dgLight))
                return 0.0;

            double pdf = Util.DistanceSquared (p, ray.Apply (thit)) / Util.AbsDot (dgLight.n, -wi) * Area;

            if (double.IsInfinity (pdf))
                pdf = 0.0;

            return pdf;
        }

        public virtual Point Sample (double u1, double u2, ref Normal Ns)
        {
            return new Point ();
        }

        public virtual Point Sample (Point p, double u1, double u2, ref Normal Ns)
        {
            return Sample (u1, u2, ref Ns);
        }

        public virtual bool Intersect (Ray ray, ref double tHit, ref double rayEpsilon, ref DifferentialGeometry dg)
        {
            return false;
        }

        public virtual bool IntersectP (Ray ray)
        {
            return false;
        }

        public virtual void GetShadingGeometry (Transform objectToWorld, DifferentialGeometry dg, ref DifferentialGeometry dgShading)
        {
            dgShading = new DifferentialGeometry (dg);
        }

        public virtual void Refine (ref List<IShape> refined)
        {
        }
    }
}
