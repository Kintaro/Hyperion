
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
            get {
                double det = ((m.m[0] * (m.m[5] * m.m[10] - m.m[6] * m.m[9])) - (m.m[1] * (m.m[4] * m.m[10] - m.m[6] * m.m[8])) + (m.m[2] * (m.m[4] * m.m[9] - m.m[5] * m.m[8])));
                return det < 0.0;
            }
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
            Transform M = new Transform (this);
            BoundingBox ret = new BoundingBox (M.Apply (new Point (b.pMin.x, b.pMin.y, b.pMin.z)));
            ret = BoundingBox.Union (ret, M.Apply (new Point (b.pMax.x, b.pMin.y, b.pMin.z)));
            ret = BoundingBox.Union (ret, M.Apply (new Point (b.pMin.x, b.pMax.y, b.pMin.z)));
            ret = BoundingBox.Union (ret, M.Apply (new Point (b.pMin.x, b.pMin.y, b.pMax.z)));
            ret = BoundingBox.Union (ret, M.Apply (new Point (b.pMin.x, b.pMax.y, b.pMax.z)));
            ret = BoundingBox.Union (ret, M.Apply (new Point (b.pMax.x, b.pMax.y, b.pMin.z)));
            ret = BoundingBox.Union (ret, M.Apply (new Point (b.pMax.x, b.pMin.y, b.pMax.z)));
            ret = BoundingBox.Union (ret, M.Apply (new Point (b.pMax.x, b.pMax.y, b.pMax.z)));
            return ret;
        }

        public static Transform operator * (Transform t1, Transform t2)
        {
            Matrix m1 = t1.m * t2.m;
            Matrix m2 = t2.mInv * t1.mInv;
            return new Transform (m1, m2);
        }

        public static Transform Translate (double x, double y, double z)
        {
            return Translate (new Vector (x, y, z));
        }

        public static Transform Translate (Vector delta)
        {
            Matrix m = new Matrix (1, 0, 0, delta.x, 0, 1, 0, delta.y, 0, 0,
            1, delta.z, 0, 0, 0, 1);
            Matrix minv = new Matrix (1, 0, 0, -delta.x, 0, 1, 0, -delta.y, 0, 0,
            1, -delta.z, 0, 0, 0, 1);
            return new Transform (m, minv);
        }

        public static Transform Scale (double x, double y, double z)
        {
            Matrix m = new Matrix (x, 0, 0, 0, 0, y, 0, 0, 0, 0,
            z, 0, 0, 0, 0, 1);
            Matrix minv = new Matrix (1.0 / x, 0, 0, 0, 0, 1.0 / y, 0, 0, 0, 0,
            1.0 / z, 0, 0, 0, 0, 1);
            return new Transform (m, minv);
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
