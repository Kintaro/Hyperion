
using System;

namespace Hyperion.Core.Reflection
{
    public class FresnelNoOp : IFresnel
    {
        public Spectrum Evaluate (double cosi)
        {
            return new Spectrum (1.0);
        }
    }
}
