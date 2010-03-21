
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public abstract class ICamera
    {
        public IFilm Film;
        public readonly double ShutterOpen;
        public readonly double ShutterClose;
        public AnimatedTransform CameraToWorld;
        public int NumberOfRays;

        public ICamera (AnimatedTransform camToWorld, double sopen, double sclose, IFilm film)
        {
            Film = film;
            sopen = ShutterOpen;
            sclose = ShutterClose;
            CameraToWorld = new AnimatedTransform (camToWorld);
        }

        public abstract double GenerateRay (CameraSample sample, ref Ray ray);

        public virtual double GenerateRayDifferential (CameraSample sample, ref RayDifferential rd)
        {
            Ray r = new Ray (rd.Origin, rd.Direction, rd.MinT, rd.MaxT, rd.Time);
            double wt = GenerateRay (sample, ref r);
            rd.Origin = r.Origin;
            rd.Direction = r.Direction;
            rd.MinT = r.MinT;
            rd.MaxT = r.MaxT;
            rd.Time = r.Time;

            CameraSample sshift = sample;
            ++sshift.ImageX;

            Ray rx = new Ray ();
            double wtx = GenerateRay (sshift, ref rx);
            rd.RxOrigin = rx.Origin;
            rd.RxDirection = rx.Direction;

            --sshift.ImageX;
            ++sshift.ImageY;

            Ray ry = new Ray ();
            double wty = GenerateRay (sshift, ref ry);
            rd.RyOrigin = ry.Origin;
            rd.RyDirection = ry.Direction;

            if (wtx == 0.0 || wty == 0.0)
                return 0.0;

            rd.HasDifferentials = true;

            return wt;
        }
    }
}
