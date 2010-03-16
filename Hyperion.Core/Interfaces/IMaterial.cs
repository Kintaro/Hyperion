
using System;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;

namespace Hyperion.Core.Interfaces
{
    public abstract class IMaterial
    {
        public virtual BSSRDF GetBssrdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading)
        {
            return null;
        }

        public abstract BSDF GetBsdf (DifferentialGeometry dgGeom, DifferentialGeometry dgShading);
    }
}
