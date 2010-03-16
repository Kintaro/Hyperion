
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public interface ITexture<T>
    {
        T Evaluate (DifferentialGeometry dg);
    }
}
