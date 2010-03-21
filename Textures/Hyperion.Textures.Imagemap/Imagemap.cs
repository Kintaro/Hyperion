
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;

namespace Hyperion.Textures.Imagemap
{
    public class Imagemap<TMem, TReturn> : ITexture<TReturn>
    {
        public Imagemap ()
        {
        }

        public TReturn Evaluate (Hyperion.Core.Geometry.DifferentialGeometry dg)
        {
            TReturn result = default(TReturn);
            return result;
        }


        public static void ClearCache ()
        {
        }

        private static void ConvertIn (Spectrum from, out Spectrum to, double scale, double gamma)
        {
            to = Spectrum.Pow (scale * from, gamma);
        }

        private static void ConvertIn (Spectrum from, out double to, double scale, double gamma)
        {
            to = Math.Pow (scale * from.y, gamma);
        }

        private static void ConvertOut (Spectrum from, out Spectrum to)
        {
            double[] rgb = new double[3];
            from.ToRgb (rgb);
            to = Spectrum.FromRgb (rgb);
        }

        private static void ConertOut (double from, out double to)
        {
            to = from;
        }
    }
}
