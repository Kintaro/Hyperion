
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;

namespace Hyperion.Cameras.Perspective
{
    public class Perspective : ProjectiveCamera
    {
        private Vector dxCamera;
        private Vector dyCamera;

        public Perspective (AnimatedTransform cameraToWorld, double[] screenWindow, double sopen, double sclose, double lensr, double focald, double fov, IFilm film)
            : base (cameraToWorld, Transform.Perspective (fov, 1e-2, 1000.0), screenWindow, sopen, sclose, lensr, focald, film)
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

    }
}
