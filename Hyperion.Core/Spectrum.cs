
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core
{
    public sealed class Spectrum
    {
        public double[] c = new double[3];

        public Spectrum () : this(0.0)
        {
        }

        public Spectrum (double v)
        {
            c[0] = v;
            c[1] = v;
            c[2] = v;
        }

        public Spectrum (double x, double y, double z)
        {
            c[0] = x;
            c[1] = y;
            c[2] = z;
        }

        public void ToRgb (double[] rgb)
        {
            rgb[0] = c[0];
            rgb[1] = c[1];
            rgb[2] = c[2];
        }

        public Spectrum Clamp ()
        {
            return Clamp (0.0, double.PositiveInfinity);
        }

        public Spectrum Clamp (double low)
        {
            return Clamp (low, double.PositiveInfinity);
        }

        public Spectrum Clamp (double low, double high)
        {
            Spectrum result = new Spectrum ();
            result.c[0] = Util.Clamp (c[0], low, high);
            result.c[1] = Util.Clamp (c[1], low, high);
            result.c[2] = Util.Clamp (c[2], low, high);
            return result;
        }

        public void ToXyz (double[] xyz)
        {
            RgbToXyz (c, xyz);
        }

        public static void RgbToXyz (double[] rgb, double[] xyz)
        {
            xyz[0] = 0.412453 * rgb[0] + 0.35758 * rgb[1] + 0.180423 * rgb[2];
            xyz[1] = 0.212671 * rgb[0] + 0.71516 * rgb[1] + 0.072169 * rgb[2];
            xyz[2] = 0.019334 * rgb[0] + 0.119193 * rgb[1] + 0.950227 * rgb[2];
        }

        public static void XyzToRgb (double[] xyz, ref double rgb0, ref double rgb1, ref double rgb2)
        {
            rgb0 = 3.240479 * xyz[0] - 1.53715 * xyz[1] - 0.498535 * xyz[2];
            rgb1 = -0.969256 * xyz[0] + 1.875991 * xyz[1] + 0.041556 * xyz[2];
            rgb2 = 0.055648 * xyz[0] - 0.204043 * xyz[1] + 1.057311 * xyz[2];
        }

        public double y {
            get {
                double[] YWeight = new double[] { 0.212671, 0.71516, 0.072169 };
                return YWeight[0] * c[0] + YWeight[1] * c[1] + YWeight[2] * c[2];
            }
        }

        public bool IsBlack {
            get { return c[0] == 0.0 && c[1] == 0.0 && c[2] == 0.0; }
        }

        public bool HasNaNs {
            get { return double.IsNaN (c[0]) || double.IsNaN (c[1]) || double.IsNaN (c[2]); }
        }

        public static Spectrum operator + (Spectrum a, Spectrum b)
        {
            Spectrum res = new Spectrum ();
            res.c[0] = a.c[0] + b.c[0];
            res.c[1] = a.c[1] + b.c[1];
            res.c[2] = a.c[2] + b.c[2];
            return res;
        }

        public static Spectrum operator - (Spectrum a, Spectrum b)
        {
            Spectrum res = new Spectrum ();
            res.c[0] = a.c[0] - b.c[0];
            res.c[1] = a.c[1] - b.c[1];
            res.c[2] = a.c[2] - b.c[2];
            return res;
        }

        public static Spectrum operator * (Spectrum s, Spectrum b)
        {
            Spectrum res = new Spectrum ();
            res.c[0] = s.c[0] * b.c[0];
            res.c[1] = s.c[1] * b.c[1];
            res.c[2] = s.c[2] * b.c[2];
            return res;
        }

        public static Spectrum operator / (Spectrum s, Spectrum b)
        {
            Spectrum res = new Spectrum ();
            res.c[0] = s.c[0] / b.c[0];
            res.c[1] = s.c[1] / b.c[1];
            res.c[2] = s.c[2] / b.c[2];
            return res;
        }

        public static Spectrum operator * (Spectrum s, double f)
        {
            Spectrum res = new Spectrum ();
            res.c[0] = s.c[0] * f;
            res.c[1] = s.c[1] * f;
            res.c[2] = s.c[2] * f;
            return res;
        }

        public static Spectrum operator * (double f, Spectrum s)
        {
            return s * f;
        }

        public static Spectrum operator / (Spectrum s, double f)
        {
            return s * (1.0 / f);
        }
    }
}
