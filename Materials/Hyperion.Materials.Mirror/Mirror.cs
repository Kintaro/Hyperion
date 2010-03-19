
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;
using Hyperion.Core.Tools;

namespace Hyperion.Materials.Mirror
{
    public class Mirror : IMaterial
    {
        private ITexture<Spectrum> Kr;
        private ITexture<double> BumpMap;

        public Mirror (ITexture<Spectrum> r, ITexture<double> bump)
        {
            Kr = r;
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
            if (!R.IsBlack)
                bsdf.Add (new SpecularReflection (R, new FresnelNoOp ()));
            return bsdf;
        }

        public static IMaterial CreateMaterial (Transform xform, TextureParameterSet mp)
        {
            ITexture<Spectrum> Kr = mp.GetSpectrumTexture ("Kr", new Spectrum (0.9));
            ITexture<double> bumpMap = mp.GetDoubleTexture ("bumpmap", 0.0);
            return new Mirror (Kr, bumpMap);
        }
    }
}
