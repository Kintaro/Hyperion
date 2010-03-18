
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

        public Vector Apply (Vector v)
        {
            double x = v.x, y = v.y, z = v.z;
            return new Vector (m.m[0] * x + m.m[1] * y + m.m[2] * z, m.m[4] * x + m.m[5] * y + m.m[6] * z, m.m[8] * x + m.m[9] * y + m.m[10] * z);
        }

        public Normal Apply (Normal n)
        {
            double x = n.x, y = n.y, z = n.z;
            return new Normal (mInv.m[0] * x + mInv.m[4] * y + mInv.m[8] * z,
                mInv.m[1] * x + mInv.m[5] * y + mInv.m[9] * z,
                mInv.m[2] * x + mInv.m[6] * y + mInv.m[10] * z);
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

        public void Apply (Vector v, ref Vector vt)
        {
            double x = v.x, y = v.y, z = v.z;
            vt.x = m.m[0] * x + m.m[1] * y + m.m[2] * z;
            vt.y = m.m[4] * x + m.m[5] * y + m.m[6] * z;
            vt.z = m.m[8] * x + m.m[9] * y + m.m[10] * z;
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

        public void Apply (Ray r, ref Ray rt)
        {
            Apply (r.Origin, ref rt.Origin);
            Apply (r.Direction, ref rt.Direction);
            if (rt != r)
            {
                rt.MinT = r.MinT;
                rt.MaxT = r.MaxT;
                rt.Time = r.Time;
                rt.Depth = r.Depth;
            }
        }

        public void Apply (RayDifferential r, ref RayDifferential rt)
        {
            Apply (r.Origin, ref rt.Origin);
            Apply (r.Direction, ref rt.Direction);
            rt.MinT = r.MinT;
            rt.MaxT = r.MaxT;
            rt.Time = r.Time;
            rt.Depth = r.Depth;

            rt.HasDifferentials = r.HasDifferentials;
            Apply (r.RxOrigin, ref rt.RxOrigin);
            Apply (r.RyOrigin, ref rt.RyOrigin);
            Apply (r.RxDirection, ref rt.RxDirection);
            Apply (r.RyDirection, ref rt.RyDirection);
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

        public static Transform Perspective (double fov, double n, double f)
        {
            Matrix persp = new Matrix (1, 0, 0, 0, 0, 1, 0, 0, 0, 0,
            f / (f - n), -f * n / (f - n), 0, 0, 1, 0);
            
            // Scale to canonical viewing volume
            double invTanAng = 1.0 / Math.Tan (Util.Radians (fov) / 2.0);
            return Scale (invTanAng, invTanAng, 1) * new Transform (persp);
        }

        public static Transform Rotate (double angle, Vector axis)
        {
            Vector a = axis.Normalized;
            double s = Math.Sin (Util.Radians (angle));
            double c = Math.Cos (Util.Radians (angle));
            double[] m = new double[16];
            
            m[0] = a.x * a.x + (1.0 - a.x * a.x) * c;
            m[1] = a.x * a.y * (1.0 - c) - a.z * s;
            m[2] = a.x * a.z * (1.0 - c) + a.y * s;
            m[3] = 0;

            m[4] = a.x * a.y * (1.0 - c) + a.z * s;
            m[5] = a.y * a.y + (1.0 - a.y * a.y) * c;
            m[6] = a.y * a.z * (1.0 - c) - a.x * s;
            m[7] = 0;

            m[8] = a.x * a.z * (1.0 - c) - a.y * s;
            m[9] = a.y * a.z * (1.0 - c) + a.x * s;
            m[10] = a.z * a.z + (1.0 - a.z * a.z) * c;
            m[11] = 0;
            
            m[12] = 0;
            m[13] = 0;
            m[14] = 0;
            m[15] = 1;
            
            Matrix mat = new Matrix (m);
            return new Transform (mat, mat.Transposed);
        }

        public static Transform LookAt (Point pos, Point look, Vector up)
        {
            double[] m = new double[16];
            // Initialize fourth column of viewing matrix
            m[3] = pos.x;
            m[7] = pos.y;
            m[11] = pos.z;
            m[15] = 1;
            
            // Initialize first three columns of viewing matrix
            Vector dir = (look - pos).Normalized;
            Vector left = (up.Normalized % dir).Normalized;
            Vector newUp = dir % left;
            m[0] = left.x;
            m[4] = left.y;
            m[8] = left.z;
            m[12] = 0.0;
            m[1] = newUp.x;
            m[5] = newUp.y;
            m[9] = newUp.z;
            m[13] = 0.0;
            m[2] = dir.x;
            m[6] = dir.y;
            m[10] = dir.z;
            m[14] = 0.0;
            Matrix camToWorld = new Matrix (m);
            return new Transform (camToWorld.Inverse, camToWorld);
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
