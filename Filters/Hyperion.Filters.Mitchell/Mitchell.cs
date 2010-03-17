
using System;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;

namespace Hyperion.Filters.Mitchell
{
    public class Mitchell : IFilter
    {
        private readonly double B;
        private readonly double C;

        public Mitchell (double b, double c, double xw, double yw) : base(xw, yw)
        {
            B = b;
            C = c;
        }

        public override double Evaluate (double x, double y)
        {
            return Mitchell1D (x * invXWidth) * Mitchell1D (y * invYWidth);
        }

        private double Mitchell1D (double x)
        {
            x = Math.Abs (2.0 * x);
            if (x > 1.0)
                return ((-B - 6 * C) * x * x * x + (6 * B + 30 * C) * x * x + (-12 * B - 48 * C) * x + (8 * B + 24 * C)) * (1.0 / 6.0);
            else
                return ((12 - 9 * B - 6 * C) * x * x * x + (-18 + 12 * B + 6 * C) * x * x + (6 - 2 * B)) * (1.0 / 6.0);
        }

        public static IFilter CreateFilter (ParameterSet ps)
        {
            double xw = ps.FindOneDouble ("xwidth", 2.0);
            double yw = ps.FindOneDouble ("ywidth", 2.0);
            double B = ps.FindOneDouble ("B", 1.0 / 3.0);
            double C = ps.FindOneDouble ("C", 1.0 / 3.0);
            return new Mitchell (B, C, xw, yw);
        }
    }
}
