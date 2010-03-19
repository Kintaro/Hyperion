
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Lights.DiffuseAreaLight
{
    public class DiffuseAreaLight : AreaLight
    {
        private Spectrum Lemit;
        private ShapeSet ShapeSet;
        private double area;

        public DiffuseAreaLight (Transform lightToWorld, Spectrum Le, int ns, IShape shape) : base(lightToWorld, ns)
        {
            Lemit = Le;
            ShapeSet = new ShapeSet (shape);
            area = ShapeSet.Area;
        }

        public override Spectrum L (Point p, Normal n, Vector w)
        {
            return (n ^ w) > 0.0 ? Lemit : new Spectrum ();
        }

        public override Spectrum SampleL (Point p, double pEpsilon, LightSample ls, double time, ref Vector wi, ref double pdf, ref VisibilityTester visibility)
        {
            Normal ns = new Normal ();
            Point ps = ShapeSet.Sample (p, ls, ref ns);
            wi = (ps - p).Normalized;
            pdf = ShapeSet.Pdf (p, wi);
            visibility.SetSegment (p, pEpsilon, ps, 0.001, time);
            Spectrum Ls = L (ps, ns, -wi);

            return Ls;
        }

        public override Spectrum Power (Scene scene)
        {
            return Lemit * area * Util.Pi;
        }

        public override double Pdf (Point p, Vector wi)
        {
            return ShapeSet.Pdf (p, wi);
        }

        public override Spectrum SampleL (Scene scene, LightSample ls, double u1, double u2, double time, ref Ray ray, ref Normal Ns, ref double pdf)
        {
            Point org = ShapeSet.Sample (ls, ref Ns);
            Vector dir = MonteCarlo.UniformSampleSphere (u1, u2);
            if ((dir ^ Ns) < 0.0)
                dir *= -1.0;
            ray = new Ray (org, dir, 0.001, double.PositiveInfinity, time);
            pdf = ShapeSet.Pdf (org) * Util.InvTwoPi;
            Spectrum Ls = L (org, Ns, dir);
            return Ls;
        }

        public override bool IsDeltaLight {
            get { return false; }
        }

        public static AreaLight CreateAreaLight (Transform lightToWorld, ParameterSet paramSet, IShape shape)
        {
            Spectrum L = paramSet.FindOneSpectrum ("L", new Spectrum (1.0));
            Spectrum sc = paramSet.FindOneSpectrum ("scale", new Spectrum (1.0));
            int nSamples = paramSet.FindOneInt ("nsamples", 1);
            return new DiffuseAreaLight (lightToWorld, L * sc, nSamples, shape);
        }
    }
}
