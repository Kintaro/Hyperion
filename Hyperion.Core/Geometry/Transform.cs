
using System;

namespace Hyperion.Core.Geometry
{
    public sealed class Transform : IComparable<Transform>
    {
        private Matrix m;
        private Matrix mInv;

        public Transform ()
        {
            m = new Matrix ();
            mInv = new Matrix ();
        }

        public Transform (double[] mat)
        {
            m = new Matrix (mat);
            mInv = new Matrix (mat).Inverse;
        }

        public Transform (Matrix matrix)
        {
            m = new Matrix (matrix);
            mInv = matrix.Inverse;
        }

        public Transform (Matrix matrix, Matrix inverse)
        {
            m = new Matrix (matrix);
            mInv = new Matrix (inverse);
        }

        public Transform (Transform transform)
        {
            m = new Matrix (transform.Matrix);
            mInv = new Matrix (transform.InverseMatrix);
        }

        public bool SwapsHandedness {
            get { return false; }
        }

        public Matrix Matrix {
            get { return m; }
        }

        public Matrix InverseMatrix {
            get { return mInv; }
        }

        public Transform Inverse {
            get { return new Transform (InverseMatrix, Matrix); }
        }

        public Point Apply (Point pt)
        {
            double x = pt.x, y = pt.y, z = pt.z;
            double xp = m.m[0] * x + m.m[1] * y + m.m[2] * z + m.m[3];
            double yp = m.m[4] * x + m.m[5] * y + m.m[6] * z + m.m[7];
            double zp = m.m[8] * x + m.m[9] * y + m.m[10] * z + m.m[11];
            double wp = m.m[12] * x + m.m[13] * y + m.m[14] * z + m.m[15];

            if (wp == 1.0)
                return new Point (xp, yp, zp);
            else
                return new Point (xp, yp, zp) / wp;
        }

        public void Apply (Point pt, ref Point ptrans)
        {
            double x = pt.x, y = pt.y, z = pt.z;
            ptrans.x = m.m[0] * x + m.m[1] * y + m.m[2] * z + m.m[3];
            ptrans.y = m.m[4] * x + m.m[5] * y + m.m[6] * z + m.m[7];
            ptrans.z = m.m[8] * x + m.m[9] * y + m.m[10] * z + m.m[11];
            double w = m.m[12] * x + m.m[13] * y + m.m[14] * z + m.m[15];

            if (w == 1.0)
                ptrans /= w;
        }

        public BoundingBox Apply (BoundingBox b)
        {
            return null;
        }

        public static bool operator == (Transform t1, Transform t2)
        {
            return t1.m == t2.m && t1.mInv == t2.mInv;
        }

        public static bool operator != (Transform t1, Transform t2)
        {
            return t1.m != t2.m || t1.mInv != t2.mInv;
        }

        public static bool operator < (Transform t1, Transform t2)
        {
            for (int i = 0; i < 16; ++i)
            {
                if (t1.m.m[i] < t2.m.m[i])
                    return true;
                if (t1.m.m[i] > t2.m.m[i])
                    return false;
            }
            return false;
        }

        public static bool operator > (Transform t1, Transform t2)
        {
            for (int i = 0; i < 16; ++i)
            {
                if (t1.m.m[i] > t2.m.m[i])
                    return true;
                if (t1.m.m[i] < t2.m.m[i])
                    return false;
            }
            return false;
        }

        public override bool Equals (object obj)
        {
            return this == obj as Transform;
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }

        public int CompareTo (Transform other)
        {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            return 0;
        }
    }
}
