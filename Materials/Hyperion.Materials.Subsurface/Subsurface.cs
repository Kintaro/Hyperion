
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;
using Hyperion.Core.Tools;

namespace Hyperion.Materials.Subsurface
{
    public class Subsurface : IMaterial
    {
        private double Scale;
        private ITexture<Spectrum> Kr;
        private ITexture<Spectrum> SigmaA;
        private ITexture<Spectrum> SigmaPrimeS;
        private ITexture<double> Eta;
        private ITexture<double> BumpMap;

        public Subsurface (double sc, ITexture<Spectrum> kr, ITexture<Spectrum> sa, ITexture<Spectrum> sps, ITexture<double> e, ITexture<double> bump)
        {
            Scale = sc;
            Kr = kr;
            SigmaA = sa;
            SigmaPrimeS = sps;
            Eta = e;
            BumpMap = bump;
        }

        public override BSDF GetBsdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading)
        {
            DifferentialGeometry dgs = new DifferentialGeometry ();
            if (BumpMap != null)
                Bump (BumpMap, dgGeom, dgShading, ref dgs);
            else
                dgs = new DifferentialGeometry (dgShading);
            BSDF bsdf = new BSDF (dgs, dgGeom.n);
            Spectrum R = Kr.Evaluate (dgs).Clamp ();
            double e = Eta.Evaluate (dgs);
            if (!R.IsBlack)
                bsdf.Add (new SpecularReflection (R, new FresnelDielectric (1.0, e)));
            return bsdf;
        }

        public override BSSRDF GetBssrdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading)
        {
            double e = Eta.Evaluate (dgShading);
            return new BSSRDF (Scale * SigmaA.Evaluate (dgShading), Scale * SigmaPrimeS.Evaluate (dgShading), e);
        }

        public static IMaterial CreateMaterial (Transform xform, TextureParameterSet mp)
        {
            double[] sa_rgb = new double[] { 0.0011, 0.0024, 0.014 }, sps_rgb = new double[] { 2.55, 3.21, 3.77 };
            Spectrum sa = Spectrum.FromRgb (sa_rgb), sps = Spectrum.FromRgb (sps_rgb);
            string name = "";
            //mp.FindString ("name");
            bool found = false;
            //GetVolumeScatteringProperties(name, &sa, &sps);
            if (name != "" && !found)
                ;
            double scale = 1.0;
            //mp.FindDouble ("scale", 1.0);
            
            ITexture<Spectrum> sigma_a, sigma_prime_s;
            sigma_a = mp.GetSpectrumTexture ("sigma_a", sa);
            sigma_prime_s = mp.GetSpectrumTexture ("sigma_prime_s", sps);
            ITexture<double> ior = mp.GetDoubleTexture ("index", 1.3f);
            ITexture<Spectrum> Kr = mp.GetSpectrumTexture ("Kr", new Spectrum (1.0));
            ITexture<double> bumpMap = mp.GetDoubleTexture ("bumpmap", 0.0);
            return new Subsurface (scale, Kr, sigma_a, sigma_prime_s, ior, bumpMap);
        }
    }
}
