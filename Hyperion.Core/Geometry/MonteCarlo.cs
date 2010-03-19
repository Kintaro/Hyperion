
using System;
using System.Collections.Generic;
using Hyperion.Core.Interfaces;

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
                } else
                {
                    // Handle second region of disk
                    r = sy;
                    theta = 2f - sx / r;
                }
            } else
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

        public static Vector UniformSampleSphere (double u1, double u2)
        {
            double z = 1.0 - 2.0 * u1;
            double r = Math.Sqrt (Math.Max (0.0, 1.0 - z * z));
            double phi = 2.0 * Util.Pi * u2;
            double x = r * Math.Cos (phi);
            double y = r * Math.Sin (phi);
            return new Vector (x, y, z);
        }

        public static double UniformSpherePdf ()
        {
            return 1.0 / (4.0 * Util.Pi);
        }

        public static void UniformSampleTriangle (double u1, double u2, out double u, out double v)
        {
            double su1 = Math.Sqrt (u1);
            u = 1.0 - su1;
            v = u2 * su1;
        }

        public static int LowDiscrepancyPixelSampleDoublesNeeded (Sample[] sample, int nPixelSamples)
        {
            int n = 5;
            // 2 lens + 2 pixel + time
            for (int i = 0; i < sample[0].n1D.Count; ++i)
                n += sample[0].n1D[i];
            for (int i = 0; i < sample[0].n2D.Count; ++i)
                n += 2 * sample[0].n2D[i];
            return nPixelSamples * n;
        }

        public static void LowDiscrepancyPixelSample (int xPos, int yPos, double shutterOpen, double shutterClose, int nPixelSamples, Sample[] samples, double[] buf)
        {
            int index = 5 * nPixelSamples;
            int ImageSamples = 0;
            int LensSamples = 2 * nPixelSamples;
            int TimeSamples = 4 * nPixelSamples;
            
            // Prepare temporary array pointers for low-discrepancy integrator samples
            int count1D = samples[0].n1D.Count;
            int count2D = samples[0].n2D.Count;
            List<int> n1D = count1D > 0 ? samples[0].n1D : null;
            List<int> n2D = count2D > 0 ? samples[0].n2D : null;
            double[][] oneDSamples = new double[count1D][];
            double[][] twoDSamples = new double[count2D][];
            
            for (int i = 0; i < count1D; ++i)
            {
                oneDSamples[i] = buf;
                index += n1D[i] * nPixelSamples;
            }
            for (int i = 0; i < count2D; ++i)
            {
                twoDSamples[i] = buf;
                index += 2 * n2D[i] * nPixelSamples;
            }
            
            // Generate low-discrepancy pixel samples
            LDShuffleScrambled2D (1, nPixelSamples, ref buf, ImageSamples);
            LDShuffleScrambled2D (1, nPixelSamples, ref buf, LensSamples);
            LDShuffleScrambled1D (1, nPixelSamples, ref buf, TimeSamples);
            
            for (int i = 0; i < count1D; ++i)
                LDShuffleScrambled1D (n1D[i], nPixelSamples, ref oneDSamples[i], 0);
            for (int i = 0; i < count2D; ++i)
                LDShuffleScrambled2D (n2D[i], nPixelSamples, ref twoDSamples[i], 0);
            
            // Initialize _samples_ with computed sample values
            for (int i = 0; i < nPixelSamples; ++i)
            {
                samples[i].ImageX = xPos + buf[ImageSamples + 2 * i];
                samples[i].ImageY = yPos + buf[ImageSamples + 2 * i + 1];
                samples[i].Time = Util.Lerp (buf[TimeSamples + i], shutterOpen, shutterClose);
                samples[i].LensU = buf[LensSamples + 2 * i];
                samples[i].LensV = buf[LensSamples + 2 * i + 1];
                // Copy integrator samples into _samples[i]_
                for (int j = 0; j < count1D; ++j)
                {
                    int startSamp = n1D[j] * i;
                    for (int k = 0; k < n1D[j]; ++k)
                        samples[i].samples[samples[i].oneD + j][k] = oneDSamples[j][startSamp + k];
                }
                for (int j = 0; j < count2D; ++j)
                {
                    int startSamp = 2 * n2D[j] * i;
                    for (int k = 0; k < 2 * n2D[j]; ++k)
                        samples[i].samples[samples[i].twoD + j][k] = twoDSamples[j][startSamp + k];
                }
            }
        }

        public static void LDShuffleScrambled1D (int numberOfSamples, int pixel, ref double[] samples, int index)
        {
            int scramble = Util.Random.Next ();
            
            for (int i = 0; i < numberOfSamples * pixel; ++i)
                samples[i + index] = VanDerCorput (i, scramble);
            for (int i = 0; i < pixel; ++i)
                Shuffle (samples, i * numberOfSamples, numberOfSamples, 1, index);
            Shuffle (samples, 0, pixel, numberOfSamples, index);
        }

        public static void LDShuffleScrambled2D (int numberOfSamples, int pixel, ref double[] samples, int index)
        {
            int[] scramble = new int[] { Util.Random.Next (), Util.Random.Next () };
            
            for (int i = 0; i < numberOfSamples * pixel; ++i)
                Sample02 (i, scramble, samples, 2 * i, index);
            for (int i = 0; i < pixel; ++i)
                Shuffle (samples, 2 * i * numberOfSamples, numberOfSamples, 2, index);
            Shuffle (samples, 0, pixel, 2 * numberOfSamples, index);
        }

        public static void Shuffle (double[] samples, int index, int count, int dims, int x)
        {
            for (int i = 0; i < count; ++i)
            {
                int other = Util.Random.Next () % count;
                for (int j = 0; j < dims; ++j)
                {
                    double temp = samples[dims * i + j];
                    samples[index + dims * i + j + x] = samples[dims * other + j + x];
                    samples[index + dims * other + j + x] = temp;
                }
            }
        }

        public static void Sample02 (int n, int[] scramble, double[] sample, int index, int x)
        {
            sample[index + 0 + x] = VanDerCorput (n, scramble[0]);
            sample[index + 1 + x] = Sobol2 (n, scramble[1]);
        }

        public static double VanDerCorput (int n, int scramble)
        {
            long t = n;
            t = (t << 16) | (t >> 16);
            t = ((t & 0xff00ff) << 8) | ((t & 0xff00ff00u) >> 8);
            t = ((t & 0xf0f0f0f) << 4) | ((t & 0xf0f0f0f0u) >> 4);
            t = ((t & 0x33333333) << 2) | ((t & 0xccccccccu) >> 2);
            t = ((t & 0x55555555) << 1) | ((t & 0xaaaaaaaau) >> 1);
            t ^= scramble;
            
            return t / 4294967296.0;
            //return (double)n / (double)BitConverter.ToDouble (BitConverter.GetBytes(0x100000000L), 0);
        }

        public static double Sobol2 (int n, int scramble)
        {
            for (int v = 1 << 31; n != 0; n >>= 1,v ^= v >> 1)
                if ((n & 0x1) != 0)
                    scramble ^= v;
            return scramble / 4294967296.0;
            //return (double)scramble / (double)BitConverter.ToDouble (BitConverter.GetBytes(0x100000000L), 0);
        }

        public static void StratifiedSample2D (double[] samp, int nx, int ny, bool jitter)
        {
            int index = 0;
            double dx = 1.0 / nx, dy = 1.0 / ny;
            for (int y = 0; y < ny; ++y)
            {
                for (int x = 0; x < nx; ++x)
                {
                    double jx = jitter ? Util.Random.NextDouble () : 0.5;
                    double jy = jitter ? Util.Random.NextDouble () : 0.5;
                    samp[index++] = (x + jx) * dx;
                    samp[index++] = (y + jy) * dy;
                }
            }
        }
    }
}
