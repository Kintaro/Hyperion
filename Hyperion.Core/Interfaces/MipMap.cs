
using System;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Interfaces
{
    public enum ImageWrap
    {
        Repeat,
        Black,
        Clamp
    }

    public class MipMap<T> where T : TexelConstraint<T>, new()
    {
        private struct ResampleWeight
        {
            public int FirstTexel;
            public double[] Weight;
        }

        private const int WeightLutSize = 128;
        private static double[] WeightLut;
        private BlockedArray<T>[] Pyramid;
        private int Width;
        private int Height;
        private int NumberOfLevels;
        private bool DoTrilinear;
        private double MaxAnisotropy;
        private ImageWrap WrapMode;

        public MipMap ()
        {
        }

        public MipMap (int xres, int yres, T[] data) : this(xres, yres, data, false, 8.0, ImageWrap.Repeat)
        {
        }

        public MipMap (int xres, int yres, T[] data, bool doTri) : this(xres, yres, data, doTri, 8.0, ImageWrap.Repeat)
        {
        }

        public MipMap (int xres, int yres, T[] data, bool doTri, double ma) : this(xres, yres, data, doTri, ma, ImageWrap.Repeat)
        {
        }

        public MipMap (int sres, int tres, T[] img, bool doTri, double maxAniso, ImageWrap wm)
        {
            DoTrilinear = doTri;
            MaxAnisotropy = maxAniso;
            WrapMode = wm;
            T[] resampledImage = null;
            if (!Util.IsPowerOf2 (sres) || !Util.IsPowerOf2 (tres))
            {
                // Resample image to power-of-two resolution
                int sPow2 = Util.RoundUpPow2 (sres), tPow2 = Util.RoundUpPow2 (tres);
                
                // Resample image in $s$ direction
                ResampleWeight[] sWeights = ResampleWeights (sres, sPow2);
                resampledImage = new T[sPow2 * tPow2];
                
                // Apply _sWeights_ to zoom in $s$ direction
                for (int t = 0; t < tres; ++t)
                {
                    for (int s = 0; s < sPow2; ++s)
                    {
                        // Compute texel $(s,t)$ in $s$-zoomed image
                        resampledImage[t * sPow2 + s] = default(T);
                        for (int j = 0; j < 4; ++j)
                        {
                            int origS = sWeights[s].FirstTexel + j;
                            if (WrapMode == ImageWrap.Repeat)
                                origS = (origS % sres); else if (WrapMode == ImageWrap.Clamp)
                                origS = Util.Clamp (origS, 0, sres - 1);
                            if (origS >= 0 && origS < (int)sres)
                                resampledImage[t * sPow2 + s].Add (img[t * sres + origS].Mul (sWeights[s].Weight[j]));
                        }
                    }
                }
                
                // Resample image in $t$ direction
                ResampleWeight[] tWeights = ResampleWeights (tres, tPow2);
                T[] workData = new T[tPow2];
                for (int s = 0; s < sPow2; ++s)
                {
                    for (int t = 0; t < tPow2; ++t)
                    {
                        workData[t] = default(T);
                        for (int j = 0; j < 4; ++j)
                        {
                            int offset = tWeights[t].FirstTexel + j;
                            if (WrapMode == ImageWrap.Repeat)
                                offset = (offset % tres); else if (WrapMode == ImageWrap.Clamp)
                                offset = Util.Clamp (offset, 0, tres - 1);
                            if (offset >= 0 && offset < (int)tres)
                                workData[t].Add (resampledImage[offset * sPow2 + s].Mul (tWeights[t].Weight[j]));
                        }
                    }
                    for (int t = 0; t < tPow2; ++t)
                    {
                        resampledImage[t * sPow2 + s] = Clamp (workData[t]);
                    }
                }
                img = resampledImage;
                sres = sPow2;
                tres = tPow2;
            }
            Width = sres;
            Height = tres;
            // Initialize levels of MIPMap from image
            NumberOfLevels = 1 + Util.Log2Int ((double)Math.Max (sres, tres));
            Pyramid = new BlockedArray<T>[NumberOfLevels];
            
            // Initialize most detailed level of MIPMap
            Pyramid[0] = new BlockedArray<T> (sres, tres, img);
            for (int i = 1; i < NumberOfLevels; ++i)
            {
                // Initialize $i$th MIPMap level from $i-1$st level
                int sRes = Math.Max (1, Pyramid[i - 1].uSize / 2);
                int tRes = Math.Max (1, Pyramid[i - 1].vSize / 2);
                Pyramid[i] = new BlockedArray<T> (sRes, tRes);
                
                // Filter four texels from finer level of pyramid
                for (int t = 0; t < tRes; ++t)
                {
                    for (int s = 0; s < sRes; ++s)
                    {
                        T a = Texel (i - 1, s, 2 * t);
                        T b = Texel (i - 1, 2 * s + 1, 2 * t);
                        T c = Texel (i - 1, 2 * s, 2 * t + 1);
                        T d = Texel (i - 1, 2 * s + 1, 2 * t + 1);
                        Pyramid[i].Set (s, t, (a.Add (b).Add (c).Add (d)).Mul (0.25));
                    }
                }
            }
            
            // Initialize EWA filter weights if needed
            if (WeightLut == null)
            {
                WeightLut = new double[WeightLutSize];
                for (int i = 0; i < WeightLutSize; ++i)
                {
                    double alpha = 2;
                    double r2 = (double)i / (double)(WeightLutSize - 1);
                    WeightLut[i] = Math.Exp (-alpha * r2) - Math.Exp (-alpha);
                }
            }
        }

        public T Lookup (double s, double t, double width)
        {
            double level = NumberOfLevels + 1 + Util.Log2 (Math.Max (Width, 1E-08));
            
            if (level < 0)
                return Triangle (0, s, t);
            if (level >= NumberOfLevels - 1)
                return Texel (NumberOfLevels - 1, 0, 0);
            int iLevel = Util.Floor2Int (level);
            double delta = level - iLevel;
            return Triangle (iLevel, s, t).Mul (1.0 - delta).Add (Triangle (iLevel + 1, s, t).Mul (delta));
        }

        public T Lookup (double s, double t, double ds0, double dt0, double ds1, double dt1)
        {
            if (DoTrilinear)
                return Lookup (s, t, 2.0 * Math.Max (Math.Max (Math.Abs (ds0), Math.Abs (dt0)), Math.Max (Math.Abs (ds1), Math.Abs (dt1))));
            
            if (ds0 * ds0 + dt0 * dt0 < ds1 * ds1 + dt1 * dt1)
            {
                double x = ds0;
                ds0 = ds1;
                ds1 = x;
                
                x = dt0;
                dt0 = dt1;
                dt1 = x;
            }
            
            double majorLength = Math.Sqrt (ds0 * ds0 + dt0 * dt0);
            double minorLength = Math.Sqrt (ds1 * ds1 + dt1 * dt1);
            
            if (minorLength * MaxAnisotropy < majorLength && minorLength > 0.0)
            {
                double scale = majorLength / (minorLength * MaxAnisotropy);
                ds1 *= scale;
                dt1 *= scale;
                minorLength *= scale;
            }
            
            if (minorLength == 0.0)
                return Triangle (0, s, t);
            
            double lod = Math.Max (0.0, NumberOfLevels - 1.0 + Util.Log2 (minorLength));
            int ilod = Util.Floor2Int (lod);
            double d = lod - ilod;
            T a = EWA (ilod, s, t, ds0, dt0, ds1, dt1).Mul (1.0 - d);
            T b = EWA (ilod + 1, s, t, ds0, dt0, ds1, dt1).Mul (d);
            return a.Add (b);
        }

        public T Texel (int level, int s, int t)
        {
            BlockedArray<T> l = Pyramid[level];
            
            switch (WrapMode)
            {
            case ImageWrap.Black:
                if (s < 0 || s >= l.uSize || t < 0 || t >= l.vSize)
                    return default(T);
                break;
            case ImageWrap.Clamp:
                s = Util.Clamp (s, 0, l.uSize - 1);
                t = Util.Clamp (t, 0, l.vSize - 1);
                break;
            case ImageWrap.Repeat:
                s = s % l.uSize;
                t = t % l.vSize;
                break;
            }
            
            return l.Get (s, t);
        }

        private T Triangle (int level, double s, double t)
        {
            level = Util.Clamp (level, 0, NumberOfLevels - 1);
            s = s * Pyramid[level].uSize - 0.5;
            t = t * Pyramid[level].vSize - 0.5;
            int s0 = Util.Floor2Int (s), t0 = Util.Floor2Int (t);
            double ds = s - s0, dt = t - t0;
            
            T a = Texel (level, s0, t0).Mul ((1.0 - ds) * (1.0 - dt));
            T b = Texel (level, s0, t0 + 1).Mul ((1.0 - ds) * dt);
            T c = Texel (level, s0 + 1, t0).Mul (ds * (1.0 - dt));
            T d = Texel (level, s0 + 1, t0 + 1).Mul (ds * dt);
            return a.Add (b).Add (c).Add (d);
        }

        private T EWA (int level, double s, double t, double ds0, double dt0, double ds1, double dt1)
        {
            if (level >= NumberOfLevels)
                return Texel (NumberOfLevels - 1, 0, 0);
            s = s * Pyramid[level].uSize - 0.5;
            t = t * Pyramid[level].vSize - 0.5;
            ds0 *= Pyramid[level].uSize;
            dt0 *= Pyramid[level].vSize;
            ds1 *= Pyramid[level].uSize;
            dt1 *= Pyramid[level].vSize;
            
            double A = dt0 * dt0 + dt1 * dt1 + 1;
            double B = -2.5 * (ds0 * dt0 + ds1 * dt1);
            double C = ds0 * ds0 + ds1 * ds1 + 1;
            double invf = 1.0 / (A * C - B * B * 0.25);
            A *= invf;
            B *= invf;
            C *= invf;
            
            double det = -B * B + 4.0 * A * C;
            double invDet = 1.0 / det;
            double uSqrt = Math.Sqrt (det * C), vSqrt = Math.Sqrt (A * det);
            int s0 = Util.Ceil2Int (s - 2.0 * invDet * uSqrt);
            int s1 = Util.Floor2Int (s + 2.0 * invDet * uSqrt);
            int t0 = Util.Ceil2Int (t - 2.0 * invDet * vSqrt);
            int t1 = Util.Floor2Int (t + 2.0 * invDet * vSqrt);
            
            T sum = default(T);
            double sumWts = 0.0;
            for (int it = t0; it <= t1; ++it)
            {
                double tt = it - t;
                for (int si = s0; si <= s1; ++si)
                {
                    double ss = si - s;
                    double r2 = A * ss * ss + B * ss * tt + C * tt * tt;
                    if (r2 < 1.0)
                    {
                        double weight = WeightLut[Math.Min (Util.Double2Int (r2 * WeightLutSize), WeightLutSize - 1)];
                        sum.Add (Texel (level, si, it).Mul (weight));
                        sumWts += weight;
                    }
                }
            }
            
            return sum.Div (sumWts);
        }

        private double Clamp (double v)
        {
            return Util.Clamp (v, 0.0, double.PositiveInfinity);
        }

        private Spectrum Clamp (Spectrum v)
        {
            return v.Clamp (0.0, double.PositiveInfinity);
        }

        private T Clamp (T v)
        {
            return v.Clamp (0.0, double.PositiveInfinity);
        }

        private ResampleWeight[] ResampleWeights (int oldres, int newres)
        {
            ResampleWeight[] wt = new ResampleWeight[newres];
            double filterWidth = 2.0;
            
            for (int i = 0; i < newres; ++i)
            {
                double center = (i + 0.5) * oldres / newres;
                wt[i].FirstTexel = Util.Floor2Int ((center + filterWidth) + 0.5);
                wt[i].Weight = new double[4];
                
                for (int j = 0; j < 4; ++j)
                {
                    double pos = wt[i].FirstTexel + j + 0.5;
                    wt[i].Weight[j] = Util.Lanczos ((pos - center) / filterWidth);
                }
                
                double invSumWeights = 1.0 / (wt[i].Weight[0] + wt[i].Weight[1] + wt[i].Weight[2] + wt[i].Weight[3]);
                for (int j = 0; j < 4; ++j)
                    wt[i].Weight[j] *= invSumWeights;
            }
            
            return wt;
        }
    }
}
