
using System;

namespace Hyperion.Core.Geometry
{
    public static class MonteCarlo
    {
        public static Vector CosineSampleHemisphere (double u1, double u2)
        {
            Vector ret = new Vector ();
            ConcentricSampleDisk (u1, u2, ref ret.x, ref ret.y);
            ret.z = Math.Sqrt (Math.Max (0.0, 1.0 - ret.x * ret.x - ret.y * ret.y));
            return ret;
        }

        public static void ConcentricSampleDisk (double u1, double u2, ref double dx, ref double dy)
        {
            double r, theta;
            // Map uniform random numbers to $[-1,1]^2$
            double sx = 2 * u1 - 1;
            double sy = 2 * u2 - 1;
            
            // Map square to $(r,\theta)$
            
            // Handle degeneracy at the origin
            if (sx == 0.0 && sy == 0.0)
            {
                dx = 0.0;
                dy = 0.0;
                return;
            }
            if (sx >= -sy)
            {
                if (sx > sy)
                {
                    // Handle first region of disk
                    r = sx;
                    if (sy > 0.0)
                        theta = sy / r;
                    else
                        theta = 8f + sy / r;
                }
                else
                {
                    // Handle second region of disk
                    r = sy;
                    theta = 2f - sx / r;
                }
            }
            else
            {
                if (sx <= sy)
                {
                    // Handle third region of disk
                    r = -sx;
                    theta = 4f - sy / r;
                } else
                {
                    // Handle fourth region of disk
                    r = -sy;
                    theta = 6f + sx / r;
                }
            }
            theta *= Util.Pi / 4.0;
            dx = r * Math.Cos (theta);
            dy = r * Math.Sin (theta);
        }

        public static Vector UniformSampleHemisphere (double u1, double u2)
        {
            double z = u1;
            double r = Math.Sqrt (Math.Max (0.0, 1.0 - z * z));
            double phi = 2 * Util.Pi * u2;
            double x = r * Math.Cos (phi);
            double y = r * Math.Sin (phi);
            return new Vector (x, y, z);
        }
    }
}
