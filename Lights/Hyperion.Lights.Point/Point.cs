
using System;
using Hyperion.Core.Interfaces;
using Hyperion.Core;

namespace Hyperion.Lights.Point
{
    public class Point : ILight
    {
        private Core.Geometry.Point LightPosition;
        private Spectrum Intensity;

        public Point (Core.Geometry.Transform lightToWorld, Spectrum intensity) : base(lightToWorld)
        {
            LightPosition = LightToWorld.Apply (new Core.Geometry.Point (0.0, 0.0, 0.0));
            Intensity = intensity;
        }

        public override Spectrum SampleL (Hyperion.Core.Geometry.Point p, double pEpsilon, LightSample ls, double time, ref Hyperion.Core.Geometry.Vector wi, ref double pdf, ref VisibilityTester visibility)
        {
            wi = (LightPosition - p).Normalized;
            pdf = 1.0;
            visibility.SetSegment (p, pEpsilon, LightPosition, 0, time);
            return Intensity / Core.Geometry.Util.DistanceSquared (LightPosition, p);
        }

        public override Spectrum SampleL (Scene scene, LightSample ls, double u1, double u2, double time, ref Hyperion.Core.Geometry.Ray ray, ref Hyperion.Core.Geometry.Normal Ns, ref double pdf)
        {
            ray = new Core.Geometry.Ray (LightPosition, Core.Geometry.MonteCarlo.UniformSampleSphere (ls.uPos[0], ls.uPos[1]), 0.0, double.PositiveInfinity, time);
            Ns = new Core.Geometry.Normal (ray.Direction);
            pdf = Core.Geometry.MonteCarlo.UniformSpherePdf ();

            return Intensity;
        }

        public override Spectrum Power (Scene scene)
        {
            return 4.0 * Core.Geometry.Util.Pi * Intensity;
        }

        public override double Pdf (Hyperion.Core.Geometry.Point p, Hyperion.Core.Geometry.Vector wi)
        {
            return 0.0;
        }

        public override void SHProject (Hyperion.Core.Geometry.Point p, double pEpsilon, int lmax, Scene scene, bool computeLightVisibility, double time, Spectrum[] coeffs)
        {
            for (int i = 0; i < Core.Geometry.SphericalHarmonics.SHTerms (lmax); ++i)
                coeffs[i] = new Spectrum ();

            if (computeLightVisibility && scene.IntersectP (new Core.Geometry.Ray (p, (LightPosition - p).Normalized, pEpsilon, Core.Geometry.Util.Distance (LightPosition, p), time)))
                return;

            double[] Ylm = new double[Core.Geometry.SphericalHarmonics.SHTerms (lmax)];
            Core.Geometry.Vector wi = (LightPosition - p).Normalized;
            //Core.Geometry.SphericalHarmonics.SHEvaluate (wi, lmax, Ylm);
            Spectrum Li = Intensity / Core.Geometry.Util.DistanceSquared (LightPosition, p);
            for (int i = 0; i < Core.Geometry.SphericalHarmonics.SHTerms (lmax); ++i)
                coeffs[i] = Li * Ylm[i];
        }

        public override bool IsDeltaLight {
            get {
                return true;
            }
        }

        public static ILight CreateLight (Core.Geometry.Transform lightToWorld, Core.Tools.ParameterSet paramSet)
        {
            Spectrum I = paramSet.FindOneSpectrum ("I", new Spectrum (1.0));
            Spectrum sc = paramSet.FindOneSpectrum ("scale", new Spectrum (1.0));
            Core.Geometry.Point P = paramSet.FindOnePoint ("from", new Core.Geometry.Point (0, 0, 0));
            Core.Geometry.Transform l2w = Core.Geometry.Transform.Translate (new Core.Geometry.Vector (P)) * lightToWorld;
            return new Point (l2w, I * sc);
        }
    }
}
