
using System;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;

namespace Hyperion.Core.Reflection
{
    public sealed class BSDFSample
    {
        public double[] uDir = new double[2];
        public double uComponent;

        public BSDFSample (double up0, double up1, double ucomp)
        {
            uDir[0] = up0;
            uDir[1] = up1;
            uComponent = ucomp;
        }

        public BSDFSample ()
        {
            uDir[0] = Util.Random.NextDouble ();
            uDir[1] = Util.Random.NextDouble ();
            uComponent = Util.Random.NextDouble ();
        }

        public BSDFSample (Sample sample, BSDFSampleOffsets offsets, int n)
        {
            uDir[0] = sample.samples[sample.twoD + offsets.DirOffset][2 * n];
            uDir[1] = sample.samples[sample.twoD + offsets.DirOffset][2 * n + 1];
            uComponent = sample.samples[sample.oneD + offsets.ComponentOffset][n];
        }
    }
}
