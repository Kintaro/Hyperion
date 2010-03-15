
using System;

namespace Hyperion.Core.Geometry
{
    [System.Runtime.InteropServices.StructLayout (System.Runtime.InteropServices.LayoutKind.Explicit)]
    public class Vector
    {
        [System.Runtime.InteropServices.FieldOffset (0)]
        public double x;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public double y;
        [System.Runtime.InteropServices.FieldOffset(16)]
        public double z;

        public Vector ()
        {
            x = y = z = 0.0;
        }

        public Vector (Vector vec) : this(vec.x, vec.y, vec.z)
        {
        }

        public Vector (double v) : this(v, v, v)
        {
        }

        public Vector (double a, double b, double c)
        {
            x = a;
            y = b;
            z = c;
        }

        public static Vector operator + (Vector a, Vector b)
        {
            return new Vector (a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector operator - (Vector a, Vector b)
        {
            return new Vector (a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector operator * (Vector a, double f)
        {
            return new Vector (a.x * f, a.y * f, a.z * f);
        }

        public static Vector operator * (double f, Vector a)
        {
            return new Vector (a.x * f, a.y * f, a.z * f);
        }

        public static Vector operator / (Vector a, double f)
        {
            return new Vector (a.x / f, a.y / f, a.z / f);
        }

        public static Vector operator - (Vector v)
        {
            return new Vector (-v.x, -v.y, -v.z);
        }

        public static double operator ^ (Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector operator % (Vector v1, Vector v2)
        {
            return new Vector ((v1.y * v2.z) - (v1.z * v2.y), (v1.z * v2.x) - (v1.x * v2.z), (v1.x * v2.y) - (v1.y * v2.x));
        }

        public static Vector operator % (Vector v1, Normal v2)
        {
            return new Vector ((v1.y * v2.z) - (v1.z * v2.y), (v1.z * v2.x) - (v1.x * v2.z), (v1.x * v2.y) - (v1.y * v2.x));
        }

        public static Vector operator % (Normal v1, Vector v2)
        {
            return new Vector ((v1.y * v2.z) - (v1.z * v2.y), (v1.z * v2.x) - (v1.x * v2.z), (v1.x * v2.y) - (v1.y * v2.x));
        }

        public Vector Normalized
        {
            get
            {
                return this / Length;
            }
        }

        public double SquaredLength
        {
            get
            {
                return this ^ this;
            }
        }

        public double Length
        {
            get
            {
                return (float)Math.Sqrt (SquaredLength);
            }
        }

        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                if (index == 1)
                    return y;
                if (index == 2)
                    return z;
                throw new IndexOutOfRangeException ();
            }
        }

        public bool HasNaNs
        {
            get
            {
                return double.IsNaN (x) || double.IsNaN (y) || double.IsNaN (z);
            }
        }
    }
}
