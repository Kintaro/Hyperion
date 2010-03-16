
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

        public ICamera (AnimatedTransform camToWorld, double sopen, double sclose, IFilm film)
        {
            Film = film;
            sopen = ShutterOpen;
            sclose = ShutterClose;
            CameraToWorld = camToWorld;
        }

        public abstract double GenerateRay (CameraSample sample, Ray ray);

        public virtual double GenerateRayDifferential (CameraSample sample, ref RayDifferential rd)
        {
            double wt = GenerateRay (sample, rd);

            CameraSample sshift = sample;
            ++sshift.ImageX;

            Ray rx = new Ray ();
            double wtx = GenerateRay (sshift, rx);
            rd.RxOrigin = rx.Origin;
            rd.RxDirection = rx.Direction;

            --sshift.ImageX;
            ++sshift.ImageY;

            Ray ry = new Ray ();
            double wty = GenerateRay (sshift, ry);
            rd.RyOrigin = ry.Origin;
            rd.RyDirection = ry.Direction;

            if (wtx == 0.0 || wty == 0.0)
                return 0.0;

            rd.HasDifferentials = true;

            return wt;
        }
    }
}
