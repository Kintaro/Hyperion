
using System;

namespace Hyperion.Core.Mappings
{
    public class UVMapping2D : ITextureMapping2D
    {
        private double su;
        private double sv;
        private double du;
        private double dv;

        public UVMapping2D () : this(1.0, 1.0, 0.0, 0.0)
        {}

        public UVMapping2D (double su) : this(su, 1.0, 0.0, 0.0)
        {
        }

        public UVMapping2D (double su, double sv) : this(su, sv, 0.0, 0.0)
        {
        }

        public UVMapping2D (double su, double sv, double du) : this (su, sv, du, 0.0)
        {}

        public UVMapping2D (double su, double sv, double du, double dv)
        {
            this.su = su;
            this.sv = sv;
            this.du = du;
            this.dv = dv;
        }

        public void Map (Hyperion.Core.Geometry.DifferentialGeometry dg, out double s, out double t, out double dsdx, out double dtdx, out double dsdy, out double dtdy)
        {
            s = su * dg.u + du;
            t = sv * dg.v + dv;
            // Compute texture differentials for 2D identity mapping
            dsdx = su * dg.dudx;
            dtdx = sv * dg.dvdx;
            dsdy = su * dg.dudy;
            dtdy = sv * dg.dvdy;
        }
    }
}
