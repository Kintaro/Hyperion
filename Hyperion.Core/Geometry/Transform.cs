
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
            mInv = null;
            // m.Inverse;
        }

        public Transform (Matrix matrix)
        {
            m = new Matrix (matrix);
            mInv = null;
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

        public bool SwapsHandedness
        {
            get
            {
                return false;
            }
        }

        public Matrix Matrix
        {
            get
            {
                return m;
            }
        }

        public Matrix InverseMatrix
        {
            get
            {
                return mInv;
            }
        }

        public Transform Inverse
        {
            get
            {
                return new Transform (InverseMatrix, Matrix);
            }
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
