
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Mappings
{
    public interface ITextureMapping2D
    {
        void Map (DifferentialGeometry dg, out double s, out double t, out double dsdx, out double dtdx, out double dsdy, out double dtdy);
    }
}
