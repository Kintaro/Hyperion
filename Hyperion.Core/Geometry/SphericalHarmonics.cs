
using System;

namespace Hyperion.Core.Geometry
{
    public static class SphericalHarmonics
    {
        public static int SHTerms (int lmax)
        {
            return (lmax + 1) * (lmax + 1);
        }
    }
}
