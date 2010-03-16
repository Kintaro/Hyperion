
using System;

namespace Hyperion.Core.Reflection
{
    public interface IFresnel
    {
        Spectrum Evaluate (double cosi);
    }
}
