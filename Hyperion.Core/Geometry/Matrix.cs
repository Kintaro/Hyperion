
using System;

namespace Hyperion.Core
{
    public sealed class Matrix
    {
        public double[] m = new double[16];

        public Matrix ()
        {
            m[0] = m[5] = m[10] = m[15] = 1.0;
        }

        public Matrix (double[] v)
        {
            v.CopyTo (m, 0);
        }

        public Matrix (double t00, double t01, double t02, double t03, double t10, double t11, double t12, double t13, double t20, double t21,
        double t22, double t23, double t30, double t31, double t32, double t33)
        {
            m[0] = t00;
            m[1] = t01;
            m[2] = t02;
            m[3] = t03;
            m[4] = t10;
            m[5] = t11;
            m[6] = t12;
            m[7] = t13;
            m[8] = t20;
            m[9] = t21;
            m[10] = t22;
            m[11] = t23;
            m[12] = t30;
            m[13] = t31;
            m[14] = t32;
            m[15] = t33;
        }

        public Matrix Transposed
        {
            get
            {
                return new Matrix (m[0], m[4], m[8], m[12],
                    m[1], m[5], m[9], m[13],
                    m[2], m[6],m[10], m[14],
                    m[3], m[7], m[11], m[15]);
            }
        }

        public static Matrix operator * (Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix ();
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    result.m[i * 4 + j] = m1.m[i * 4 + 0] * m2.m[0 * 4 + j] +
                            m1.m[i * 4 + 1] * m2.m[4 + j] +
                            m1.m[i * 4 + 2] * m2.m[8 + j] +
                            m1.m[i * 4 + 3] * m2.m[12 + j];
                }
            }
            return result;
        }
    }
}
