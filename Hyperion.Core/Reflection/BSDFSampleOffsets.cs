
using System;
using Hyperion.Core.Interfaces;

namespace Hyperion.Core.Reflection
{
    public struct BSDFSampleOffsets
    {
        public int nSamples;
        public int ComponentOffset;
        public int DirOffset;

        public BSDFSampleOffsets (int count, Sample sample)
        {
            nSamples = count;
            ComponentOffset = sample.Add1D (nSamples);
            DirOffset = sample.Add2D (nSamples);
        }
    }
}
