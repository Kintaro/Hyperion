
using System;

namespace Hyperion.Core.Reflection
{
    public sealed class FresnelConductor : IFresnel
    {
        private Spectrum Eta;
        private Spectrum K;

        public FresnelConductor (Spectrum e, Spectrum k)
        {
        }

        public Spectrum Evaluate (double cosi)
        {
            return ReflectionUtil.FrCond (Math.Abs (cosi), Eta, K);
        }

    }
}
