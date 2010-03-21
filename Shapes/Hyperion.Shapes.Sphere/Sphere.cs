
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;

namespace Hyperion.Shapes.Sphere
{
    public class Sphere : IShape
    {
        private double Radius;
        private double PhiMax;
        private double zMin;
        private double zMax;
        private double ThetaMin;
        private double ThetaMax;

        public Sphere (Transform objectToWorld, Transform worldToObject, bool reverse, double radius, double zmin, double zmax, double phiMax) : base(objectToWorld, worldToObject, reverse)
        {
            Radius = radius;
            zMin = Util.Clamp (Math.Min (zmin, zmax), -Radius, Radius);
            zMax = Util.Clamp (Math.Max (zmin, zmax), -Radius, Radius);
            ThetaMin = Math.Acos (Util.Clamp (zMin / Radius, -1.0, 1.0));
            ThetaMax = Math.Acos (Util.Clamp (zMax / Radius, -1.0, 1.0));
            PhiMax = Util.Radians (Util.Clamp (phiMax, 0.0, 360.0));
        }

        public override double Pdf (Point p, Vector wi)
        {
            Point pCenter = ObjectToWorld.Apply (new Point ());
            if (Util.DistanceSquared (p, pCenter) - Radius * Radius < 0.0001)
                return base.Pdf (p, wi);
            double sinThetaMax2 = Radius * Radius / Util.DistanceSquared (p, pCenter);
            double cosThetaMax = Math.Sqrt (Math.Max (0.0, 1.0 - sinThetaMax2));
            
            return MonteCarlo.UniformConePdf (cosThetaMax);
        }

        public override Point Sample (double u1, double u2, ref Normal Ns)
        {
            Point p = new Point () + Radius * MonteCarlo.UniformSampleSphere (u1, u2);
            Ns = ObjectToWorld.Apply (new Normal (p.x, p.y, p.z)).Normalized;
            if (ReverseOrientation)
                Ns *= -1.0;
            return ObjectToWorld.Apply (p);
        }

        public override Point Sample (Point p, double u1, double u2, ref Normal Ns)
        {
            // Compute coordinate system for sphere sampling
            Point Pcenter = ObjectToWorld.Apply (new Point ());
            Vector wc = (Pcenter - p).Normalized;
            Vector wcX, wcY;
            Util.CoordinateSystem (wc, out wcX, out wcY);
            
            // Sample uniformly on sphere if $\pt{}$ is inside it
            if (Util.DistanceSquared (p, Pcenter) - Radius * Radius < 0.0001)
                return Sample (u1, u2, ref Ns);
            
            // Sample sphere uniformly inside subtended cone
            double sinThetaMax2 = Radius * Radius / Util.DistanceSquared (p, Pcenter);
            double cosThetaMax = Math.Sqrt (Math.Max (0.0, 1.0 - sinThetaMax2));
            DifferentialGeometry dgSphere = new DifferentialGeometry ();
            double thit = 0.0, rayEpsilon = 0.0;
            Point ps;
            Ray r = new Ray (p, MonteCarlo.UniformSampleCone (u1, u2, cosThetaMax, wcX, wcY, wc), 0.001);
            if (!Intersect (r, ref thit, ref rayEpsilon, ref dgSphere))
                thit = ((Pcenter - p) ^ r.Direction.Normalized);
            ps = r.Apply (thit);
            Ns = new Normal ((ps - Pcenter).Normalized);
            if (ReverseOrientation)
                Ns *= -1.0;
            return ps;
        }

        public override bool Intersect (Ray r, ref double tHit, ref double rayEpsilon, ref DifferentialGeometry dg)
        {
            double phi;
            Point phit;
            // Transform _Ray_ to object space
            Ray ray = new Ray ();
            WorldToObject.Apply (r, ref ray);
            
            // Compute quadratic sphere coefficients
            double A = ray.Direction.x * ray.Direction.x + ray.Direction.y * ray.Direction.y + ray.Direction.z * ray.Direction.z;
            double B = 2 * (ray.Direction.x * ray.Origin.x + ray.Direction.y * ray.Origin.y + ray.Direction.z * ray.Origin.z);
            double C = ray.Origin.x * ray.Origin.x + ray.Origin.y * ray.Origin.y + ray.Origin.z * ray.Origin.z - Radius * Radius;
            
            // Solve quadratic equation for _t_ values
            double t0 = 0.0, t1 = 0.0;
            if (!Util.Quadratic (A, B, C, ref t0, ref t1))
                return false;
            
            // Compute intersection distance along ray
            if (t0 > ray.MaxT || t1 < ray.MinT)
                return false;
            double thit = t0;
            if (t0 < ray.MinT)
            {
                thit = t1;
                if (thit > ray.MaxT)
                    return false;
            }
            
            // Compute sphere hit position and $\phi$
            phit = ray.Apply (thit);
            if (phit.x == 0.0 && phit.y == 0.0)
                phit.x = 1E-5 * Radius;
            phi = Math.Atan2 (phit.y, phit.x);
            if (phi < 0.0)
                phi += 2.0 * Util.Pi;
            
            // Test sphere intersection against clipping parameters
            if ((zMin > -Radius && phit.z < zMin) || (zMax < Radius && phit.z > zMax) || phi > PhiMax)
            {
                if (thit == t1)
                    return false;
                if (t1 > ray.MaxT)
                    return false;
                thit = t1;
                // Compute sphere hit position and $\phi$
                phit = ray.Apply (thit);
                if (phit.x == 0.0 && phit.y == 0.0)
                    phit.x = 1E-5f * Radius;
                phi = Math.Atan2 (phit.y, phit.x);
                if (phi < 0.0)
                    phi += 2.0 * Util.Pi;
                if ((zMin > -Radius && phit.z < zMin) || (zMax < Radius && phit.z > zMax) || phi > PhiMax)
                    return false;
            }
            
            // Find parametric representation of sphere hit
            double u = phi / PhiMax;
            double theta = Math.Acos (Util.Clamp (phit.z / Radius, -1.0, 1.0));
            double v = (theta - ThetaMin) / (ThetaMax - ThetaMin);
            
            // Compute sphere $\dpdu$ and $\dpdv$
            double zRadius = Math.Sqrt (phit.x * phit.x + phit.y * phit.y);
            double invzRadius = 1.0 / zRadius;
            double cosphi = phit.x * invzRadius;
            double sinphi = phit.y * invzRadius;
            Vector dpdu = new Vector (-PhiMax * phit.y, PhiMax * phit.x, 0);
            Vector dpdv = (ThetaMax - ThetaMin) * new Vector (phit.z * cosphi, phit.z * sinphi, -Radius * Math.Sin (theta));
            
            // Compute sphere $\dndu$ and $\dndv$
            Vector d2Pduu = -PhiMax * PhiMax * new Vector (phit.x, phit.y, 0);
            Vector d2Pduv = (ThetaMax - ThetaMin) * phit.z * PhiMax * new Vector (-sinphi, cosphi, 0.0);
            Vector d2Pdvv = -(ThetaMax - ThetaMin) * (ThetaMax - ThetaMin) * new Vector (phit.x, phit.y, phit.z);
            
            // Compute coefficients for fundamental forms
            double E = (dpdu ^ dpdu);
            double F = (dpdu ^ dpdv);
            double G = (dpdv ^ dpdv);
            Vector N = (dpdu % dpdv).Normalized;
            double e = (N ^ d2Pduu);
            double f = (N ^ d2Pduv);
            double g = (N ^ d2Pdvv);
            
            // Compute $\dndu$ and $\dndv$ from fundamental form coefficients
            double invEGF2 = 1.0 / (E * G - F * F);
            Normal dndu = new Normal ((f * F - e * G) * invEGF2 * dpdu + (e * F - f * E) * invEGF2 * dpdv);
            Normal dndv = new Normal ((g * F - f * G) * invEGF2 * dpdu + (f * F - g * E) * invEGF2 * dpdv);
            
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
            double phi;
            Point phit;
            // Transform _Ray_ to object space
            Ray ray = new Ray ();
            WorldToObject.Apply (r, ref ray);
            
            // Compute quadratic sphere coefficients
            double A = ray.Direction.x * ray.Direction.x + ray.Direction.y * ray.Direction.y + ray.Direction.z * ray.Direction.z;
            double B = 2 * (ray.Direction.x * ray.Origin.x + ray.Direction.y * ray.Origin.y + ray.Direction.z * ray.Origin.z);
            double C = ray.Origin.x * ray.Origin.x + ray.Origin.y * ray.Origin.y + ray.Origin.z * ray.Origin.z - Radius * Radius;

            // Solve quadratic equation for _t_ values
            double t0 = 0.0, t1 = 0.0;
            if (!Util.Quadratic (A, B, C, ref t0, ref t1))
                return false;

            // Compute intersection distance along ray
            if (t0 > ray.MaxT || t1 < ray.MinT)
                return false;
            double thit = t0;
            if (t0 < ray.MinT)
            {
                thit = t1;
                if (thit > ray.MaxT)
                    return false;
            }
            
            // Compute sphere hit position and $\phi$
            phit = ray.Apply (thit);
            if (phit.x == 0.0 && phit.y == 0.0)
                phit.x = 1E-5 * Radius;
            phi = Math.Atan2 (phit.y, phit.x);
            if (phi < 0.0)
                phi += 2.0 * Util.Pi;
            
            // Test sphere intersection against clipping parameters
            if ((zMin > -Radius && phit.z < zMin) || (zMax < Radius && phit.z > zMax) || phi > PhiMax)
            {
                if (thit == t1)
                    return false;
                if (t1 > ray.MaxT)
                    return false;
                thit = t1;
                // Compute sphere hit position and $\phi$
                phit = ray.Apply (thit);
                if (phit.x == 0.0 && phit.y == 0.0)
                    phit.x = 1E-5 * Radius;
                phi = Math.Atan2 (phit.y, phit.x);
                if (phi < 0.0)
                    phi += 2.0 * Util.Pi;
                if ((zMin > -Radius && phit.z < zMin) || (zMax < Radius && phit.z > zMax) || phi > PhiMax)
                    return false;
            }
            return true;
        }

        public override BoundingBox ObjectBound {
            get { return new BoundingBox (new Point (-Radius, -Radius, zMin), new Point (Radius, Radius, zMax)); }
        }

        public override double Area {
            get { return PhiMax * Radius * (zMax - zMin); }
        }

        public static IShape CreateShape (Transform o2w, Transform w2o, bool reverseOrientation, ParameterSet parameters, Dictionary<string, ITexture<double>> floatTextures)
        {
            double radius = parameters.FindOneDouble ("radius", 1.0);
            double zmin = parameters.FindOneDouble ("zmin", -radius);
            double zmax = parameters.FindOneDouble ("zmax", radius);
            double phimax = parameters.FindOneDouble ("phimax", 360.0);
            return new Sphere (o2w, w2o, reverseOrientation, radius, zmin, zmax, phimax);
        }
    }
}
