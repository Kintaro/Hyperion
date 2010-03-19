
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Reflection;

namespace Hyperion.Materials.Measured
{
    public class Measured : IMaterial
    {
        private double[] RegularHalfangleData;
        private int nThetaH;
        private int nThetaD;
        private int nPhiD;
        private ITexture<double> BumpMap;

        public Measured (string filename, ITexture<double> bump)
        {
            BumpMap = bump;
        }

        public override BSDF GetBsdf (Hyperion.Core.Geometry.DifferentialGeometry dgGeom, Hyperion.Core.Geometry.DifferentialGeometry dgShading)
        {
            throw new System.NotImplementedException ();
        }

    }
}
