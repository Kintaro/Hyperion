
using System;
using System.Collections.Generic;
using Hyperion.Core.Geometry;

namespace Hyperion.Core
{
    public class Spectrum : Interfaces.TexelConstraint<Spectrum>
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

        public static Spectrum FromRgb (double[] rgb)
        {
            Spectrum s = new Spectrum ();
            s.c[0] = rgb[0];
            s.c[1] = rgb[1];
            s.c[2] = rgb[2];
            return s;
        }

        public static Spectrum FromXyz (double[] xyz)
        {
            Spectrum r = new Spectrum ();
            XyzToRgb (xyz, ref r.c[0], ref r.c[1], ref r.c[2]);
            return r;
        }


        public static Spectrum Pow (Spectrum s, double e)
        {
            Spectrum result = new Spectrum ();
            result.c[0] = Math.Pow (s.c[0], e);
            result.c[1] = Math.Pow (s.c[1], e);
            result.c[2] = Math.Pow (s.c[2], e);
            return result;
        }

        public override string ToString ()
        {
            return string.Format ("[Spectrum: {0}, {1}, {2}]", c[0], c[1], c[2]);
        }

        public Spectrum Add (Spectrum v)
        {
            return this + v;
        }

        public Spectrum Div (double f)
        {
            return this / f;
        }

        public Spectrum Mul (double f)
        {
            return this * f;
        }

        public Spectrum Mul (Spectrum x, double f)
        {
            return x * f;
        }

        public Spectrum Pow (Spectrum x, double a, double e)
        {
            return Spectrum.Pow (x * a, e);
        }

        public void Set (double val)
        {
            c[0] = c[1] = c[2] = val;
        }

        public void SetSpectrum (Spectrum s)
        {
            c[0] = s.c[0];
            c[1] = s.c[1];
            c[2] = s.c[2];
        }

        public static Spectrum FromSampled (double[] lambda, double[] v, int n)
        {
            // Sort samples if unordered, use sorted for returned spectrum
            if (!SpectrumSamplesSorted (lambda, v, n))
            {
                List<double> slambda = new List<double> (lambda);
                List<double> sv = new List<double> (v);
                SortSpectrumSamples (ref slambda, ref sv, n);
                return FromSampled (slambda.ToArray (), sv.ToArray (), n);
            }
            double[] xyz = new double[] { 0, 0, 0 };
            double yint = 0.0;
            for (int i = 0; i < 471; ++i)
            {
                yint += SpectrumCIE.CIE_Y[i];
                double val = InterpolateSpectrumSamples (lambda, v, n, SpectrumCIE.CIE_Lambda[i]);
                xyz[0] += val * SpectrumCIE.CIE_X[i];
                xyz[1] += val * SpectrumCIE.CIE_Y[i];
                xyz[2] += val * SpectrumCIE.CIE_Z[i];
            }
            xyz[0] /= yint;
            xyz[1] /= yint;
            xyz[2] /= yint;
            return FromXyz (xyz);
        }

        public static bool SpectrumSamplesSorted (double[] lambda, double[] vals, int n)
        {
            for (int i = 0; i < n - 1; ++i)
                if (lambda[i] >= lambda[i + 1])
                    return false;
            return true;
        }

        public static void SortSpectrumSamples (ref List<double> lambda, ref List<double> vals, int n)
        {
            List<KeyValuePair<double, double>> sortVec = new List<KeyValuePair<double, double>> (n);
            for (int i = 0; i < n; ++i)
                sortVec.Add (new KeyValuePair<double, double> (lambda[i], vals[i]));
            sortVec.Sort ();
            for (int i = 0; i < n; ++i)
            {
                lambda[i] = sortVec[i].Key;
                vals[i] = sortVec[i].Value;
            }
        }

        public static double InterpolateSpectrumSamples (double[] lambda, double[] vals, int n, double l)
        {
            if (l <= lambda[0])
                return vals[0];
            if (l >= lambda[n - 1])
                return vals[n - 1];
            for (int i = 0; i < n - 1; ++i)
            {
                if (l >= lambda[i] && l <= lambda[i + 1])
                {
                    double t = (l - lambda[i]) / (lambda[i + 1] - lambda[i]);
                    return Util.Lerp (t, vals[i], vals[i + 1]);
                }
            }
            return 0.0;
        }
    }
}
