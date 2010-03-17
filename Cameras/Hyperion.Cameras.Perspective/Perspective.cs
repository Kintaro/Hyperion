
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Cameras.Perspective
{
    public class Perspective : ProjectiveCamera
    {
        private Vector dxCamera;
        private Vector dyCamera;

        public Perspective (AnimatedTransform cameraToWorld, double[] screenWindow, double sopen, double sclose, double lensr, double focald, double fov, IFilm film) : base(cameraToWorld, Transform.Perspective (fov, 0.01, 1000.0), screenWindow, sopen, sclose, lensr, focald, film)
        {
            dxCamera = RasterToCamera.Apply (new Point (1.0, 0.0, 0.0)) - RasterToCamera.Apply (new Point (0.0, 0.0, 0.0));
            dyCamera = RasterToCamera.Apply (new Point (0.0, 1.0, 0.0)) - RasterToCamera.Apply (new Point (0.0, 0.0, 0.0));
        }

        public override double GenerateRay (CameraSample sample, Ray ray)
        {
            // Generate raster and camera samples
            Point pRas = new Point (sample.ImageX, sample.ImageY, 0);
            Point pCam = new Point ();
            RasterToCamera.Apply (pRas, ref pCam);
            
            ray.Origin = new Point ();
            ray.Direction = new Vector (pCam);
            ray.MinT = 0.0;
            ray.MaxT = double.PositiveInfinity;
            
            if (LensRadius > 0.0)
            {
                double lensU = 0.0, lensV = 0.0;
                MonteCarlo.ConcentricSampleDisk (sample.LensU, sample.LensV, ref lensU, ref lensV);
                lensU *= LensRadius;
                lensV *= LensRadius;
                
                double ft = FocalDistance / ray.Direction.z;
                Point pFocus = ray.Apply (ft);
                
                ray.Origin = new Point (lensU, lensV, 0.0);
                ray.Direction = (pFocus - ray.Origin).Normalized;
            }
            
            ray.Time = Util.Lerp (sample.Time, ShutterOpen, ShutterClose);
            CameraToWorld.Apply (ray, ref ray);
            
            return 1.0;
        }

        public override double GenerateRayDifferential (CameraSample sample, ref RayDifferential rd)
        {
            Point pRas = new Point (sample.ImageX, sample.ImageY, 0.0);
            Point pCam = new Point ();
            RasterToCamera.Apply (pRas, ref pCam);
            Vector dir = new Vector (pCam).Normalized;
            rd = new RayDifferential (new Point (), dir, 0.0, double.PositiveInfinity);
            
            if (LensRadius > 0.0)
            {
                double lensU = 0.0, lensV = 0.0;
                MonteCarlo.ConcentricSampleDisk (sample.LensU, sample.LensV, ref lensU, ref lensV);
                lensU *= LensRadius;
                lensV *= LensRadius;
                
                double ft = FocalDistance / rd.Direction.z;
                Point pFocus = rd.Apply (ft);
                
                rd.Origin = new Point (lensU, lensV, 0.0);
                rd.Direction = (pFocus - rd.Origin).Normalized;
            }
            
            rd.RxOrigin = new Point (rd.Origin);
            rd.RyOrigin = new Point (rd.Origin);
            rd.RxDirection = (new Vector (pCam) + dxCamera).Normalized;
            rd.RyDirection = (new Vector (pCam) + dyCamera).Normalized;
            rd.Time = Util.Lerp (sample.Time, ShutterOpen, ShutterClose);
            CameraToWorld.Apply (rd, ref rd);
            rd.HasDifferentials = true;
            return 1.0;
        }

        public static ICamera CreateCamera (ParameterSet parameters, AnimatedTransform cameraToWorld, IFilm film)
        {
            double shutteropen = parameters.FindOneDouble ("shutteropen", 0.0);
            double shutterclose = parameters.FindOneDouble ("shutterclose", 1.0);
            double lensradius = parameters.FindOneDouble ("lensradius", 0.0);
            double focaldistance = parameters.FindOneDouble ("focaldistance", 1E+30);
            double frame = parameters.FindOneDouble ("frameaspectratio", (double)(film.xResolution) / (double)(film.yResolution));
            double[] screen = new double[4];
            if (frame > 1.0)
            {
                screen[0] = -frame;
                screen[1] = frame;
                screen[2] = -1.0;
                screen[3] = 1.0;
            } else
            {
                screen[0] = -1.0;
                screen[1] = 1.0;
                screen[2] = -1.0 / frame;
                screen[3] = 1.0 / frame;
            }
            int swi = 0;
            double[] sw = parameters.FindDouble ("screenwindow", ref swi);
            if (sw != null && swi == 4)
                screen = sw;
            double fov = parameters.FindOneDouble ("fov", 90.0);
            return new Perspective (cameraToWorld, screen, shutteropen, shutterclose, lensradius, focaldistance, fov, film);
        }
    }
}
