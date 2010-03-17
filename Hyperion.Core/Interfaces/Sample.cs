
using System;
using System.Collections.Generic;

namespace Hyperion.Core.Interfaces
{
    public class Sample : CameraSample
    {
        public List<int> n1D = new List<int> ();
        public List<int> n2D = new List<int> ();
        public int oneD;
        public int twoD;
        public double[][] samples;

        public Sample ()
        {}

        public Sample (ISampler sampler, ISurfaceIntegrator surface, IVolumeIntegrator volume, Scene scene)
        {
            if (surface != null)
                surface.RequestSamples (sampler, this, scene);
            if (volume != null)
                volume.RequestSamples (sampler, this, scene);
            AllocateSampleMemory ();
        }

        public void AllocateSampleMemory ()
        {
            int nPtrs = n1D.Count + n2D.Count;

            if (nPtrs == 0)
            {
                samples = null;
                return;
            }

            samples = new double[nPtrs][];
            oneD = 0;
            twoD = oneD + n1D.Count;

            int totSamples = 0;
            for (int i = 0; i < n1D.Count; ++i)
                totSamples += n1D[i];
            for (int i = 0; i < n2D.Count; ++i)
                totSamples += n2D[i];

            for (int i = 0; i < n1D.Count; ++i)
            {
                samples[oneD + i] = new double[n1D[i]];
            }

            for (int i = 0; i < n2D.Count; ++i)
            {
                samples[twoD + i] = new double[2 * n2D[i]];
            }
        }

        public int Add1D (int num)
        {
            n1D.Add (num);
            return n1D.Count - 1;
        }

        public int Add2D (int num)
        {
            n2D.Add (num);
            return n2D.Count - 1;
        }

        public Sample[] Duplicate (int count)
        {
            Sample[] ret = new Sample[count];

            for (int i = 0; i < count; ++i)
            {
                ret[i] = new Sample ();
                ret[i].n1D = new List<int> (n1D);
                ret[i].n2D = new List<int> (n2D);
                ret[i].AllocateSampleMemory ();
            }

            return ret;
        }
    }
}
