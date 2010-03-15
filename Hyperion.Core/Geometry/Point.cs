
using System;

namespace Hyperion.Core.Geometry
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public class Point
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public double x;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public double y;
        [System.Runtime.InteropServices.FieldOffset(16)]
        public double z;

        public Point ()
        {
            x = y = z = 0.0;
        }

        public Point (Point p) : this(p.x, p.y, p.z)
        {
        }

        public Point (double v) : this(v, v, v)
        {
        }

        public Point (double a, double b, double c)
        {
            x = a;
            y = b;
            z = c;
        }

        public static Point operator + (Point a, Point b)
        {
            return new Point (a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Point operator + (Point a, Vector b)
        {
            return new Point (a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector operator - (Point a, Point b)
        {
            return new Vector (a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Point operator - (Point a, Vector b)
        {
            return new Point (a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Point operator * (Point a, double f)
        {
            return new Point (a.x * f, a.y * f, a.z * f);
        }

        public static Point operator * (double f, Point a)
        {
            return new Point (a.x * f, a.y * f, a.z * f);
        }

        public static Point operator / (Point a, double f)
        {
            return new Point (a.x / f, a.y / f, a.z / f);
        }

        public static Point operator - (Point v)
        {
            return new Point (-v.x, -v.y, -v.z);
        }

        public static double operator ^ (Point a, Point b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public Point Pointized {
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
