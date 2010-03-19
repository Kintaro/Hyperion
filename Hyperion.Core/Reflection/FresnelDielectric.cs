
using System;
using Hyperion.Core;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Reflection
{
    public class FresnelDielectric : IFresnel
    {
        private double EtaI;
        private double EtaT;

        public FresnelDielectric (double ei, double et)
        {
            EtaI = ei;
            EtaT = et;
        }

        public Spectrum Evaluate (double cosi)
        {
            cosi = Util.Clamp (cosi, -1.0, 1.0);
            
            // Compute indices of refraction for dielectric
            bool entering = cosi > 0.0;
            double ei = EtaI, et = EtaT;
            if (!entering)
            {
                double t = ei;
                ei = et;
                et = t;
            }
            
            // Compute _sint_ using Snell's law
            double sint = ei / et * Math.Sqrt (Math.Max (0.0, 1.0 - cosi * cosi));
            if (sint >= 1.0)
            {
                // Handle total internal reflection
                return new Spectrum (1.0);
            } else
            {
                double cost = Math.Sqrt (Math.Max (0.0, 1.0 - sint * sint));
                return ReflectionUtil.FrDiel (Math.Abs (cosi), cost, new Spectrum (ei), new Spectrum (et));
            }
        }
        
    }
}
