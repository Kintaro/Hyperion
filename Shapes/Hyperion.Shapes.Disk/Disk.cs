
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Shapes.Disk
{
    public class Disk : IShape
    {
        private double Height;
        private double Radius;
        private double InnerRadius;
        private double PhiMax;

        public Disk (Transform objectToWorld, Transform worldToObject, bool reverseOrientation, double height, double radius, double innerRadius, double phiMax) : base(objectToWorld, worldToObject, reverseOrientation)
        {
            Height = height;
            Radius = radius;
            InnerRadius = innerRadius;
            PhiMax = phiMax;
        }

        public override Point Sample (double u1, double u2, ref Normal Ns)
        {
            Point p = new Point ();
            MonteCarlo.ConcentricSampleDisk (u1, u2, ref p.x, ref p.y);
            p.x *= Radius;
            p.y *= Radius;
            p.z = Height;
            Ns = ObjectToWorld.Apply (new Normal (0, 0, 1)).Normalized;
            if (ReverseOrientation)
                Ns *= -1.0;
            return ObjectToWorld.Apply (p);
        }

        public override bool Intersect (Ray r, ref double tHit, ref double rayEpsilon, ref DifferentialGeometry dg)
        {
            // Transform _Ray_ to object space
            Ray ray = new Ray ();
            WorldToObject.Apply (r, ref ray);
            
            // Compute plane intersection for disk
            if (Math.Abs (ray.Direction.z) < 1E-07)
                return false;
            double thit = (Height - ray.Origin.z) / ray.Direction.z;
            if (thit < ray.MinT || thit > ray.MaxT)
                return false;
            
            // See if hit point is inside disk radii and $\phimax$
            Point phit = ray.Apply (thit);
            double dist2 = phit.x * phit.x + phit.y * phit.y;
            if (dist2 > Radius * Radius || dist2 < InnerRadius * InnerRadius)
                return false;
            
            // Test disk $\phi$ value against $\phimax$
            double phi = Math.Atan2 (phit.y, phit.x);
            if (phi < 0)
                phi += 2.0 * Util.Pi;
            if (phi > PhiMax)
                return false;
            
            // Find parametric representation of disk hit
            double u = phi / PhiMax;
            double v = 1.0 - ((Math.Sqrt (dist2) - InnerRadius) / (Radius - InnerRadius));
            Vector dpdu = new Vector (-PhiMax * phit.y, PhiMax * phit.x, 0.0);
            Vector dpdv = new Vector (-phit.x / (1 - v), -phit.y / (1 - v), 0.0);
            dpdu *= PhiMax * Util.InvTwoPi;
            dpdv *= (Radius - InnerRadius) / Radius;
            Normal dndu = new Normal (0, 0, 0), dndv = new Normal (0, 0, 0);
            
            // Initialize _DifferentialGeometry_ from parametric information
            dg = new DifferentialGeometry (ObjectToWorld.Apply (phit), ObjectToWorld.Apply (dpdu), ObjectToWorld.Apply (dpdv), ObjectToWorld.Apply (dndu), ObjectToWorld.Apply (dndv), u, v, this);
            
            // Update _tHit_ for quadric intersection
            tHit = thit;
            
            // Compute _rayEpsilon_ for quadric intersection
            rayEpsilon = 0.0005 * tHit;
            return true;
        }

        public override bool IntersectP (Ray r)
        {
            // Transform _Ray_ to object space
            Ray ray = new Ray ();
            WorldToObject.Apply (r, ref ray);
            
            // Compute plane intersection for disk
            if (Math.Abs (ray.Direction.z) < 1E-07)
                return false;
            double thit = (Height - ray.Origin.z) / ray.Direction.z;
            if (thit < ray.MinT || thit > ray.MaxT)
                return false;
            
            // See if hit point is inside disk radii and $\phimax$
            Point phit = ray.Apply (thit);
            double dist2 = phit.x * phit.x + phit.y * phit.y;
            if (dist2 > Radius * Radius || dist2 < InnerRadius * InnerRadius)
                return false;
            
            // Test disk $\phi$ value against $\phimax$
            double phi = Math.Atan2 (phit.y, phit.x);
            if (phi < 0)
                phi += 2.0 * Util.Pi;
            if (phi > PhiMax)
                return false;
            return true;
        }


        public override double Area {
            get { return PhiMax * 0.5 * (Radius * Radius - InnerRadius * InnerRadius); }
        }

        public override BoundingBox ObjectBound {
            get { return new BoundingBox (new Point (-Radius, -Radius, Height), new Point (Radius, Radius, Height)); }
        }

        public static IShape CreateShape (Transform o2w, Transform w2o, bool reverseOrientation, ParameterSet parameters, Dictionary<string, ITexture<double>> floatTextures, Dictionary<string, ITexture<Spectrum>> spectrumTextures)
        {
            double height = parameters.FindOneDouble ("height", 0.0);
            double radius = parameters.FindOneDouble ("radius", 1);
            double inner_radius = parameters.FindOneDouble ("innerradius", 0);
            double phimax = parameters.FindOneDouble ("phimax", 360);
            return new Disk (o2w, w2o, reverseOrientation, height, radius, inner_radius, phimax);
        }
    }
}
