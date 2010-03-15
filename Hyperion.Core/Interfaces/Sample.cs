
using System;
using System.Collections.Generic;

namespace Hyperion.Core.Interfaces
{
    public class Sample : CameraSample
    {
        public List<int> n1D;
        public List<int> n2D;
        public double[][] oneD;
        public double[][] twoD;

        public Sample (ISampler sampler, ISurfaceIntegrator surface, IVolumeIntegrator volume, Scene scene)
        {
        }
    }
}
