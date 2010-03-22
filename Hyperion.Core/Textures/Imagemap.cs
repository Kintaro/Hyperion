
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Mappings;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Textures
{
    public class Imagemap<TMem, TReturn> : ITexture<TReturn> where TMem : TexelConstraint<TMem>, new()
        where TReturn : TexelConstraint<TReturn>, new()
    {
        private static Dictionary<TexInfo, MipMap<TMem>> Textures = new Dictionary<TexInfo, MipMap<TMem>> ();
        private MipMap<TMem> Mipmap;
        private ITextureMapping2D Mapping;

        public Imagemap (ITextureMapping2D m, string filename, bool doTrilinear, double maxAniso, ImageWrap wrapMode, double scale, double gamma)
        {
            Mapping = m;
            Mipmap = GetTexture (filename, doTrilinear, maxAniso, wrapMode, scale, gamma);
        }

        public TReturn Evaluate (Hyperion.Core.Geometry.DifferentialGeometry dg)
        {
            double s, t, dsdx, dtdx, dsdy, dtdy;
            Mapping.Map (dg, out s, out t, out dsdx, out dtdx, out dsdy, out dtdy);
            TMem mem = Mipmap.Lookup (s, t, dsdx, dtdx, dsdy, dtdy);
            TReturn result;
            if (mem as Spectrum != null)
                ConvertOutSpectrum (mem as Spectrum, out result);
            else
                ConvertOutSpectrum (mem as Spectrum, out result);
            return result;
        }

        public static void ClearCache ()
        {
        }

        private static double ConvertIn (Spectrum @from, double scale, double gamma, double to)
        {
            return Math.Pow (scale * @from.y, gamma);
        }

        private static Spectrum ConvertInSpectrum (Spectrum @from, double scale, double gamma)
        {
            return Spectrum.Pow (scale * @from, gamma);
        }

        private static void ConvertOutSpectrum (Spectrum @from, out TReturn to)
        {
            double[] rgb = new double[3];
            @from.ToRgb (rgb);
            Spectrum temp = Spectrum.FromRgb (rgb);
            to = new TReturn ();
            to.SetSpectrum (temp);
        }

        private static void ConvertOutDouble (double @from, out TReturn to)
        {
            to = new TReturn ();
            to.SetSpectrum (new Spectrum (from));
        }

        private static MipMap<TMem> GetTexture (string filename, bool doTrilinear, double maxAniso, ImageWrap wrap, double scale, double gamma)
        {
            Console.WriteLine ("  > Loading Texture [{0}]", filename);
            TexInfo texInfo = new TexInfo (filename, doTrilinear, maxAniso, wrap, scale, gamma);
            if (Textures.ContainsKey (texInfo))
                return Textures[texInfo];
            int width, height;

            Spectrum[] texels = ImageIo.ReadImage (filename, out width, out height);
            Console.WriteLine ("    - Width:  {0}", width);
            Console.WriteLine ("    - Height: {0}", height);
            MipMap<TMem> ret = null;
            if (texels != null)
            {
                // Convert texels to type _Tmemory_ and create _MIPMap_
                TMem[] convertedTexels = new TMem[width * height];
                for (int i = 0; i < width * height; ++i)
                {
                    convertedTexels[i] = new TMem ();
                    if (texels[i] as Spectrum != null)
                        convertedTexels[i].SetSpectrum(ConvertInSpectrum (texels[i], scale, gamma));
                    /*if (convertedTexels[i] as TexelDouble != null)
                        convertedTexels[i] = ConvertIn (texels[i], scale, gamma, convertedTexels[i] as Spectrum);*/
                }
                ret = new MipMap<TMem> (width, height, convertedTexels, doTrilinear, maxAniso, wrap);
            } else
            {
                // Create one-valued _MIPMap_
                TMem[] oneVal = new TMem[1];
                oneVal[0] = oneVal[0].Pow (oneVal[0], scale, gamma);
                ret = new MipMap<TMem> (1, 1, oneVal);
            }
            Textures[texInfo] = ret;
            return ret;
        }

        public static ITexture<Spectrum> CreateSpectrumTexture (Transform texToWorld, TextureParameterSet textureParameters)
        {
            ITextureMapping2D map = null;
            string type = textureParameters.FindString ("mapping", "uv");
            /*if (type == "uv")
            {
                double su = textureParameters.FindDouble ("uscale", 1.0);
                double sv = textureParameters.FindDouble ("vscale", 1.0);
                double tu = textureParameters.FindDouble ("udelta", 0.0);
                double tv = textureParameters.FindDouble ("vdelta", 0.0);
                map = new UVMapping2D (su, sv, tu, tv);
            }
            else if (type == "spherical")
                map = new SphericalMapping (texToWorld);
            else*/
                map = new UVMapping2D ();

            /*double maxAniso = textureParameters.FindDouble ("maxanisotropy", 8.0);
            bool trilerp = textureParameters.FindBool ("trilinear", false);*/
            double maxAniso = 8.0;
            bool trilerp = false;
            ImageWrap wrapMode = ImageWrap.Repeat;
            double scale = 1.0;
            double gamma = 1.0;
            return new Imagemap<Spectrum, Spectrum> (map, textureParameters.FindString ("filename"), trilerp, maxAniso, wrapMode, scale, gamma);
        }
    }
}
