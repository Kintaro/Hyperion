
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public abstract class AreaLight : ILight
    {
        public AreaLight (Transform lightToWorld, int ns) : base(lightToWorld, ns)
        {
        }

        public abstract Spectrum L (Point p, Normal n, Vector w);
    }
}
