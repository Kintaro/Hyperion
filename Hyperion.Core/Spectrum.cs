
using System;

namespace Hyperion.Core
{
    public sealed class Spectrum
    {
        public double[] c = new double[3];

        public Spectrum () : this (0.0)
        {}

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

        public double y
        {
            get
            {
                double[] YWeight = new double[] { 0.212671, 0.715160, 0.072169 };
                return YWeight[0] * c[0] + YWeight[1] * c[1] + YWeight[2] * c[2];
            }
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
