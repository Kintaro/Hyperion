
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;
using Hyperion.Core.Tools;

namespace Hyperion.Materials.Matte
{
    public class Matte : IMaterial
    {
        private ITexture<Spectrum> Kd;
        private ITexture<double> Sigma;
        private ITexture<double> BumpMap;

        public Matte (ITexture<Spectrum> Kd, ITexture<double> Sigma, ITexture<double> BumpMap)
        {
            this.Kd = Kd;
            this.Sigma = Sigma;
            this.BumpMap = BumpMap;
        }

        public override BSDF GetBsdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading)
        {
            DifferentialGeometry dgs = new DifferentialGeometry ();
            if (BumpMap != null)
                Bump (BumpMap, dgGeom, dgShading, ref dgs);
            else
                dgs = new DifferentialGeometry (dgShading);

            BSDF bsdf = new BSDF (dgs, dgGeom.n);
            Spectrum r = Kd.Evaluate (dgs).Clamp ();
            double sig = Util.Clamp (Sigma.Evaluate (dgs), 0.0, 90.0);
            if (sig == 0.0)
                bsdf.Add (new Lambertian (r));
            else
                bsdf.Add (new OrenNayar (r, sig));

            return bsdf;
        }

        public static IMaterial CreateMaterial (Transform xform, TextureParameterSet mp)
        {
            ITexture<Spectrum> Kd = mp.GetSpectrumTexture ("Kd", new Spectrum (0.5));
            ITexture<double> sigma = mp.GetDoubleTexture ("sigma", 0.0);
            ITexture<double> bumpMap = mp.GetDoubleTexture ("bumpmap", 0.0);
            return new Matte (Kd, sigma, bumpMap);
        }
    }
}
