
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;
using Hyperion.Core.Tools;

namespace Hyperion.Materials.Glass
{
    public class Glass : IMaterial
    {
        private ITexture<Spectrum> Kr;
        private ITexture<Spectrum> Kt;
        private ITexture<double> Index;
        private ITexture<double> BumpMap;

        public Glass (ITexture<Spectrum> r, ITexture<Spectrum> t, ITexture<double> i, ITexture<double> bump)
        {
            BumpMap = bump;
            Index = i;
            Kt = t;
            Kr = r;
        }

        public override BSDF GetBsdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading)
        {
            DifferentialGeometry dgs = new DifferentialGeometry ();
            if (BumpMap != null)
                Bump (BumpMap, dgGeom, dgShading, ref dgs);
            else
                dgs = new DifferentialGeometry (dgShading);
            
            double ior = Index.Evaluate (dgs);
            BSDF bsdf = new BSDF (dgs, dgGeom.n, ior);
            Spectrum R = Kr.Evaluate (dgs).Clamp ();
            Spectrum T = Kt.Evaluate (dgs).Clamp ();
            
            if (!R.IsBlack)
                bsdf.Add (new SpecularReflection (R, new FresnelDielectric (1.0, ior)));
            if (!T.IsBlack)
                bsdf.Add (new SpecularTransmission (T, 1.0, ior));
            
            return bsdf;
        }

        public static IMaterial CreateMaterial (Transform xform, TextureParameterSet mp)
        {
            ITexture<Spectrum> Kr = mp.GetSpectrumTexture ("Kr", new Spectrum (1.0));
            ITexture<Spectrum> Kt = mp.GetSpectrumTexture ("Kt", new Spectrum (1.0));
            ITexture<double> index = mp.GetDoubleTexture ("index", 1.50);
            ITexture<double> bumpMap = mp.GetDoubleTexture ("bumpmap", 0.0);
            return new Glass (Kr, Kt, index, bumpMap);
        }
    }
}
