
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{


    public interface IVolumeRegion
    {
        BoundingBox WorldBound { get; }
    }
}
