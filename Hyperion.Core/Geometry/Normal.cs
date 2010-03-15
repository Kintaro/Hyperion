
using System;

namespace Hyperion.Core.Geometry
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public class Normal
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public double x;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public double y;
        [System.Runtime.InteropServices.FieldOffset(16)]
        public double z;

        public Normal ()
        {
            x = y = z = 0.0;
        }

        public Normal (Normal vec) : this(vec.x, vec.y, vec.z)
        {
        }

        public Normal (Vector vec) : this(vec.x, vec.y, vec.z)
        {
        }

        public Normal (double v) : this(v, v, v)
        {
        }

        public Normal (double a, double b, double c)
        {
            x = a;
            y = b;
            z = c;
        }

        public static Normal operator + (Normal a, Normal b)
        {
            return new Normal (a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Normal operator - (Normal a, Normal b)
        {
            return new Normal (a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Normal operator * (Normal a, double f)
        {
            return new Normal (a.x * f, a.y * f, a.z * f);
        }

        public static Normal operator * (double f, Normal a)
        {
            return new Normal (a.x * f, a.y * f, a.z * f);
        }

        public static Normal operator / (Normal a, double f)
        {
            return new Normal (a.x / f, a.y / f, a.z / f);
        }

        public static Normal operator - (Normal v)
        {
            return new Normal (-v.x, -v.y, -v.z);
        }

        public static double operator ^ (Normal a, Normal b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public Normal Normalized {
            get { return this / Length; }
        }

        public double SquaredLength {
            get { return this ^ this; }
        }

        public double Length {
            get { return (float)Math.Sqrt (SquaredLength); }
        }

        public double this[int index] {
            get {
                if (index == 0)
                    return x;
                if (index == 1)
                    return y;
                if (index == 2)
                    return z;
                throw new IndexOutOfRangeException ();
            }
        }

        public bool HasNaNs {
            get { return double.IsNaN (x) || double.IsNaN (y) || double.IsNaN (z); }
        }
    }
}
