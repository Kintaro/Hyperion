
using System;

namespace Hyperion.Core.Geometry
{
    public class AnimatedTransform
    {
        private readonly double StartTime;
        private readonly double EndTime;
        private readonly bool ActuallyAnimated;
        private readonly Transform StartTransform;
        private readonly Transform EndTransform;
        private Vector[] T;
        private Quaternion[] R;
        private Matrix[] S;

        public AnimatedTransform (Transform transform1, double time1, Transform transform2, double time2)
        {
            StartTime = time1;
            EndTime = time2;
            StartTransform = new Transform (transform1);
            EndTransform = new Transform (transform2);
            ActuallyAnimated = StartTransform != EndTransform;
            
            T = new Vector[] { new Vector (), new Vector () };
            R = new Quaternion[] { new Quaternion (), new Quaternion () };
            S = new Matrix[] { new Matrix (), new Matrix () };
            
            Decompose (StartTransform.Matrix, ref T[0], ref R[0], ref S[0]);
            Decompose (StartTransform.Matrix, ref T[1], ref R[1], ref S[1]);
        }

        public static void Decompose (Matrix m, ref Vector T, ref Quaternion Rquat, ref Matrix S)
        {
            T.x = m.m[3];
            T.y = m.m[7];
            T.z = m.m[11];
            
            // Compute new transformation matrix _M_ without translation
            Matrix M = new Matrix (m);
            for (int i = 0; i < 3; ++i)
                M.m[i * 4 + 3] = M.m[12 + i] = 0.0;
            M.m[15] = 1.0;
            
            // Extract rotation _R_ from transformation matrix
            double norm;
            int count = 0;
            Matrix R = new Matrix (M);
            do
            {
                // Compute next matrix _Rnext_ in series
                Matrix Rnext = new Matrix ();
                Matrix Rit = R.Transposed.Inverse;
                for (int i = 0; i < 4; ++i)
                    for (int j = 0; j < 4; ++j)
                        Rnext.m[i * 4 + j] = 0.5 * (R.m[i * 4 + j] + Rit.m[i * 4 + j]);
                
                // Compute norm of difference between _R_ and _Rnext_
                norm = 0.0;
                for (int i = 0; i < 3; ++i)
                {
                    double n = Math.Abs (R.m[i * 4] - Rnext.m[i * 4]) + Math.Abs (R.m[i * 4 + 1] - Rnext.m[i * 4 + 1]) + Math.Abs (R.m[i * 4 + 2] - Rnext.m[i * 4 + 2]);
                    norm = Math.Max (norm, n);
                }
                R = Rnext;
            } while (++count < 100 && norm > 0.0001);
            // XXX TODO FIXME deal with flip...
            Rquat = new Quaternion (new Transform (R));
            
            // Compute scale _S_ using rotation and original matrix
            S = new Matrix (R.Inverse * M);
        }

        public void Interpolate (double time, ref Transform t)
        {
            if (!ActuallyAnimated || time <= StartTime)
            {
                t = new Transform (StartTransform);
                return;
            }
            if (time >= EndTime)
            {
                t = new Transform (EndTransform);
                return;
            }

            double dt = (time - StartTime) / (EndTime - StartTime);

            // Interpolate translation at dt
            Vector translation = (1.0 - dt) * T[0] + dt * T[1];

            // Interpolate rotation at dt
            Quaternion rotation = Quaternion.Slerp (dt, R[0], R[1]);

            // Interpolate scale at dt
            Matrix scale = new Matrix ();
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    scale.m[i * 4 + j] = Util.Lerp (dt, S[0].m[i * 4 + j], S[1].m[i * 4 + j]);

            t = Transform.Translate (translation) * rotation.Transform * new Transform (scale);
        }
    }
}
