
using System;

namespace Hyperion.Core.Interfaces
{
    public interface TexelConstraint<T>
    {
        T Add (T v);
        T Mul (double f);
        T Mul (T x, double f);
        T Div (double f);
        void Set (double val);
        T Clamp (double low, double high);
        T Pow (T x, double a, double e);
        void SetSpectrum (Spectrum s);
    }
}
