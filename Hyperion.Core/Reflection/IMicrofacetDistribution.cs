
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public interface IMicrofacetDistribution
    {
        double D (Vector wh);
        void SampleF (Vector wo, ref Vector wi, double u1, double u2, ref double pdf);
        double Pdf (Vector wo, Vector wi);
    }
}
