
using System;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Interfaces
{
    public class MipMap<T> where T : TexelConstraint<T>, new ()
    {
        private BlockedArray<T>[] Pyramid;
        private int Width;
        private int Height;
        private int NumberOfLevels;
        private bool DoTrilinear;
        private double MaxAnisotropy;

        public MipMap ()
        {
        }

        public T Texel (int level, int s, int t)
        {
            BlockedArray<T> l = Pyramid[level];
            return l.Get (s, t);
        }

        private T Triangle (int level, double s, double t)
        {
            level = Util.Clamp (level, 0, NumberOfLevels - 1);
            s = s * Pyramid[level].uSize - 0.5;
            t = t * Pyramid[level].vSize - 0.5;
            int s0 = Util.Floor2Int (s), t0 = Util.Floor2Int (t);
            double ds = s - s0, dt = t - t0;

            return Texel (level, s0, t0).Mul ((1.0 - ds) * (1.0 - dt)).Add (Texel (level, s0, t0 + 1).Mul ((1.0 - ds) * dt)).Add (Texel (level, s0 + 1, t0).Mul (ds * (1.0 - dt))).Add (Texel (level, s0 + 1, t0 + 1).Mul (ds * dt));
        }

        private T EWA (int level, double s, double t, double ds0, double dt0, double ds1, double dt1)
        {
            return default(T);
        }

        private double Clamp (double v)
        {
            return Util.Clamp (v, 0.0, double.PositiveInfinity);
        }

        private Spectrum Clamp (Spectrum v)
        {
            return v.Clamp (0.0, double.PositiveInfinity);
        }
    }
}
