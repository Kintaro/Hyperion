
using System;
using System.Collections.Generic;

namespace Hyperion.Core.Geometry
{
    public sealed class Distribution1D
    {
        // Distribution1D Private Data
        double[] func, cdf;
        double funcInt;
        int count;
        public Distribution1D (List<double> f, int n)
        {
            count = n;
            func = new double[n];
            f.CopyTo (func, 0);
            
            cdf = new double[n + 1];
            // Compute integral of step function at $x_i$
            cdf[0] = 0.0;
            for (int i = 1; i < n + 1; ++i)
                cdf[i] = cdf[i - 1] + f[i - 1] / n;
            
            // Transform step function integral into CDF
            funcInt = cdf[n];
            for (int i = 1; i < n + 1; ++i)
                cdf[i] /= funcInt;
        }

        public double SampleContinuous (double u, ref double pdf)
        {
            // Find surrounding CDF segments and _offset_
            int index = Array.BinarySearch (cdf, 0, count + 1, u);
            int offset = Math.Max (0, (int)(index - 1));
            
            // Compute offset along CDF segment
            double du = (u - cdf[offset]) / (cdf[offset + 1] - cdf[offset]);

            // Compute PDF for sampled offset
            pdf = func[offset] / funcInt;
            
            // Return $x\in{}[0,1)$ corresponding to sample
            return (offset + du) / count;
        }

        public int SampleDiscrete (double u, ref double pdf)
        {
            // Find surrounding CDF segments and _offset_
            int index = Array.BinarySearch (cdf, 0, count + 1, u);
            int offset = Math.Max (0, (int)(index - 1));
            pdf = func[offset] / (funcInt * count);
            return offset;
        }
    }
}
