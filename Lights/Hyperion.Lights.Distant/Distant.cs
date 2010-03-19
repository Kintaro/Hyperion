
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Lights.Distant
{
    public class Distant : ILight
    {
        private Vector LightDir;
        private Spectrum L;

        public Distant (Transform lightToWorld, Spectrum radiance, Vector direction) : base(lightToWorld)
        {
            LightDir = new Vector (direction);
            L = radiance;
        }

        public override Spectrum SampleL (Point p, double pEpsilon, LightSample ls, double time, ref Vector wi, ref double pdf, ref VisibilityTester visibility)
        {
            wi = new Vector (LightDir);
            pdf = 1.0;
            visibility.SetRay (p, pEpsilon, wi, time);
            return L;
        }

        public override Spectrum Power (Scene scene)
        {
            Point worldCenter;
            double worldRadius;
            scene.WorldBound.BoundingSphere (out worldCenter, out worldRadius);
            return L * Util.Pi * worldRadius * worldRadius;
        }

        public override double Pdf (Point p, Vector wi)
        {
            return 0.0;
        }

        public override Spectrum SampleL (Scene scene, LightSample ls, double u1, double u2, double time, ref Ray ray, ref Normal Ns, ref double pdf)
        {
            // Choose point on disk oriented toward infinite light direction
            Point worldCenter;
            double worldRadius;
            scene.WorldBound.BoundingSphere (out worldCenter, out worldRadius);
            Vector v1, v2;
            Util.CoordinateSystem (LightDir, out v1, out v2);
            double d1 = 0.0, d2 = 0.0;
            MonteCarlo.ConcentricSampleDisk (ls.uPos[0], ls.uPos[1], ref d1, ref d2);
            Point Pdisk = worldCenter + worldRadius * (d1 * v1 + d2 * v2);
            
            // Set ray origin and direction for infinite light ray
            ray = new Ray (Pdisk + worldRadius * LightDir, -LightDir, 0.0, double.PositiveInfinity, time);
            Ns = new Normal (ray.Direction);
            pdf = 1.0 / (Util.Pi * worldRadius * worldRadius);
            return L;
        }

        public override bool IsDeltaLight {
            get { return true; }
        }

        public static ILight CreateLight (Transform light2world, ParameterSet paramSet)
        {
            Spectrum L = paramSet.FindOneSpectrum ("L", new Spectrum (1.0));
            Spectrum sc = paramSet.FindOneSpectrum ("scale", new Spectrum (1.0));
            Point @from = paramSet.FindOnePoint ("from", new Point (0, 0, 0));
            Point to = paramSet.FindOnePoint ("to", new Point (0, 0, 1));
            Vector dir = @from - to;
            return new Distant (light2world, L * sc, dir);
        }
    }
}
