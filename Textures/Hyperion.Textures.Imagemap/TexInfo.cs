
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;

namespace Hyperion.Textures.Imagemap
{
    public class TexInfo
    {
        public string Filename;
        public bool DoTrilinear;
        public double MaxAniso;
        public double Scale;
        public double Gamma;

        public TexInfo ()
        {
        }

        public static bool operator < (TexInfo a, TexInfo b)
        {
            if (a.Filename != b.Filename)
                return string.Compare (a.Filename, b.Filename) == -1;
            if (a.DoTrilinear != b.DoTrilinear)
                return !a.DoTrilinear;
            if (a.MaxAniso != b.MaxAniso)
                return a.MaxAniso < b.MaxAniso;
            if (a.Scale != b.Scale)
                return a.Scale < b.Scale;
            if (a.Gamma != b.Gamma)
                return a.Gamma < b.Gamma;
            return false;
        }

        public static bool operator > (TexInfo a, TexInfo b)
        {
            if (a.Filename != b.Filename)
                return string.Compare (a.Filename, b.Filename) == 1;
            if (a.DoTrilinear != b.DoTrilinear)
                return a.DoTrilinear;
            if (a.MaxAniso != b.MaxAniso)
                return a.MaxAniso > b.MaxAniso;
            if (a.Scale != b.Scale)
                return a.Scale > b.Scale;
            if (a.Gamma != b.Gamma)
                return a.Gamma > b.Gamma;
            return false;
        }
    }
}
