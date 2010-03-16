
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public class LightSample
    {
        public double[] uPos;
        public double uComponent;

        public LightSample ()
        {
            uPos[0] = Util.Random.NextDouble ();
            uPos[1] = Util.Random.NextDouble ();
            uComponent = Util.Random.NextDouble ();
        }
    }
}
