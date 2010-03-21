
using System;

namespace Hyperion.Core.Interfaces
{
    public interface TexelConstraint<T>
    {
        T Add (T v);
        T Mul (double f);
        void Set (double val);
    }
}
