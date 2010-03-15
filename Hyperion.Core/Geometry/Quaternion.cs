
using System;

namespace Hyperion.Core.Geometry
{
    public sealed class Quaternion
    {
        public Vector v;
        public double w;

        public Quaternion ()
        {
            v = new Vector ();
            w = 1.0;
        }

        public Quaternion (Vector v, double w)
        {
            this.v = new Vector (v);
            this.w = w;
        }

        public Quaternion (Transform t)
        {
            Matrix m = t.Matrix;
            double trace = m.m[0] + m.m[5] + m.m[10];
            if (trace > 0.0)
            {
                // Compute w from matrix trace, then xyz
                // 4w^2 = m[0][0] + m[1][1] + m[2][2] + m[3][3] (but m[3][3] == 1)
                double s = Math.Sqrt (trace + 1.0);
                w = s / 2.0;
                s = 0.5 / s;
                v.x = (m.m[9] - m.m[6]) * s;
                v.y = (m.m[2] - m.m[8]) * s;
                v.z = (m.m[4] - m.m[1]) * s;
            }
            else
            {
                // Compute largest of $x$, $y$, or $z$, then remaining components
                int[] nxt = new int[] { 1, 2, 0 };
                double[] q = new double[3];
                int i = 0;
                if (m.m[5] > m.m[0])
                    i = 1;
                if (m.m[10] > m.m[i * 4 + i])
                    i = 2;
                int j = nxt[i];
                int k = nxt[j];
                double s = Math.Sqrt ((m.m[i * 4 + i] - (m.m[j * 4 + j] + m.m[k * 4 + k])) + 1.0);
                q[i] = s * 0.5;
                if (s != 0.0)
                    s = 0.5 / s;
                w = (m.m[k * 4 + j] - m.m[j * 4 + k]) * s;
                q[j] = (m.m[j * 4 + i] + m.m[i * 4 + j]) * s;
                q[k] = (m.m[k * 4 + i] + m.m[i * 4 + k]) * s;
                v.x = q[0];
                v.y = q[1];
                v.z = q[2];
            }
        }

        public Transform Transform {
            get {
                double xx = v.x * v.x, yy = v.y * v.y, zz = v.z * v.z;
                double xy = v.x * v.y, xz = v.x * v.z, yz = v.y * v.z;
                double wx = v.x * w, wy = v.y * w, wz = v.z * w;
                
                Matrix m = new Matrix ();
                m.m[0] = 1.0 - 2.0 * (yy + zz);
                m.m[1] = 2.0 * (xy + wz);
                m.m[2] = 2.0 * (xz - wy);
                m.m[4] = 2.0 * (xy - wz);
                m.m[5] = 1.0 - 2.0 * (xx + zz);
                m.m[6] = 2.0 * (yz + wx);
                m.m[8] = 2.0 * (xz + wy);
                m.m[9] = 2.0 * (yz - wx);
                m.m[10] = 1.0 - 2.0 * (xx + yy);
                
                // Transpose since we are left-handed.  Ugh.
                return new Transform (m.Transposed, m);
            }
        }

        public static Quaternion operator + (Quaternion a, Quaternion b)
        {
            return new Quaternion (a.v + b.v, a.w + b.w);
        }

        public static Quaternion operator - (Quaternion a, Quaternion b)
        {
            return new Quaternion (a.v - b.v, a.w - b.w);
        }

        public static Quaternion operator * (Quaternion a, double f)
        {
            return new Quaternion (a.v * f, a.w * f);
        }

        public static Quaternion operator / (Quaternion a, double f)
        {
            return new Quaternion (a.v / f, a.w / f);
        }
    }
}
