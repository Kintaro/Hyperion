
using System;

namespace Hyperion.Core.Reflection
{
    public static class ReflectionUtil
    {
        public static Spectrum FrCond (double cosi, Spectrum eta, Spectrum k)
        {
            Spectrum tmp = (eta * eta + k * k) * cosi * cosi;
            Spectrum Rparl2 = (tmp - (2.0 * eta * cosi) + new Spectrum (1.0)) / (tmp + (2.0 * eta * cosi) + new Spectrum (1.0));
            Spectrum tmp_f = eta * eta + k * k;
            Spectrum Rperp2 = (tmp_f - (2.0 * eta * cosi) + new Spectrum (cosi * cosi)) / (tmp_f + (2.0 * eta * cosi) + new Spectrum (cosi * cosi));
            return (Rparl2 + Rperp2) / 2.0;
        }
    }
}
