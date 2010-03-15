
using System;

namespace Hyperion.Core.Interfaces
{
    public abstract class IFilter
    {
        public readonly double xWidth;
        public readonly double yWidth;
        public readonly double invXWidth;
        public readonly double invYWidth;

        public IFilter (double xw, double yw)
        {
            xWidth = xw;
            yWidth = yw;
            invXWidth = 1.0 / xWidth;
            invYWidth = 1.0 / yWidth;
        }

        public abstract double Evaluate (double x, double y);
    }
}
