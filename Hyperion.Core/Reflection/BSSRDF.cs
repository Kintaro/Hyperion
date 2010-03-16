
using System;

namespace Hyperion.Core.Reflection
{
    public class BSSRDF
    {
        private double e;
        private Spectrum SigA;
        private Spectrum SigpS;

        public BSSRDF (Spectrum sa, Spectrum ps, double et)
        {
            e = et;
            SigA = sa;
            SigpS = ps;
        }

        public double Eta
        {
            get
            {
                return e;
            }
        }

        public Spectrum SigmaA
        {
            get
            {
                return SigA;
            }
        }

        public Spectrum SigmaPrimeS
        {
            get
            {
                return SigpS;
            }
        }
    }
}
