
using Hyperion.Core.Interfaces;
using Hyperion.Core.Reflection;

namespace Hyperion.Core.Interfaces
{
    public abstract class IAggregate : IPrimitive
    {
        public override BSDF GetBsdf (Hyperion.Core.Geometry.DifferentialGeometry dg, Hyperion.Core.Geometry.Transform objectoToWorld)
        {
            return null;
        }

        public override BSSRDF GetBssrdf (Hyperion.Core.Geometry.DifferentialGeometry dg, Hyperion.Core.Geometry.Transform objectToWorld)
        {
            return null;
        }

        public override AreaLight AreaLight {
            get {
                return null;
            }
        }
    }
}
