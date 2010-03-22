
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;
using Hyperion.Core.Tools;

namespace Hyperion.Materials.MixedMaterial
{
    public class MixedMaterial : IMaterial
    {
        private IMaterial m1;
        private IMaterial m2;
        private ITexture<Spectrum> scale;

        public MixedMaterial (IMaterial mat1, IMaterial mat2, ITexture<Spectrum> sc)
        {
            m1 = mat1;
            m2 = mat2;
            scale = sc;
        }

        public override BSDF GetBsdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading)
        {
            BSDF b1 = m1.GetBsdf (dgGeom, dgShading);
            BSDF b2 = m2.GetBsdf (dgGeom, dgShading);
            Spectrum s1 = scale.Evaluate (dgShading).Clamp ();
            Spectrum s2 = (new Spectrum (1.0) - s1).Clamp ();
            int n1 = b1.NumComponents (), n2 = b2.NumComponents ();
            for (int i = 0; i < n1; ++i)
                b1.bxdfs[i] = new ScaledBxDF (b1.bxdfs[i], s1);
            for (int i = 0; i < n2; ++i)
                b1.Add (new ScaledBxDF (b2.bxdfs[i], s2));
            return b1;
        }

        public static IMaterial CreateMaterial (Transform xform, TextureParameterSet mp)
        {
            string m1 = mp.FindMaterialString ("first", "");
            string m2 = mp.FindMaterialString ("second", "");
            IMaterial mat1 = Api.GraphicsState.NamedMaterials[m1];
            IMaterial mat2 = Api.GraphicsState.NamedMaterials[m2];

            ITexture<Spectrum> scale = mp.GetSpectrumTexture ("amount", new Spectrum (0.5));
            return new MixedMaterial (mat1, mat2, scale);
        }
    }
}
