
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{


    public abstract class ProjectiveCamera : ICamera
    {
        protected Transform CameraToScreen;
        protected Transform RasterToCamera;
        protected Transform ScreenToRaster;
        protected Transform RasterToScreen;
        protected double LensRadius;
        protected double FocalDistance;

        public ProjectiveCamera (AnimatedTransform cameraToWorld, Transform projection, double[] screenWindow, double sopen, double sclose, double lensr, double focald, IFilm film)
            : base(cameraToWorld, sopen, sclose, film)
        {
            LensRadius = lensr;
            FocalDistance = focald;
            CameraToScreen = new Transform (projection);

            ScreenToRaster = Transform.Scale (film.xResolution, film.yResolution, 1.0) *
                Transform.Scale (1.0 / (screenWindow[1] - screenWindow[0]), 1.0 / (screenWindow[2] - screenWindow[3]), 1.0) *
                    Transform.Translate (-screenWindow[0], -screenWindow[3], 0.0);
            RasterToScreen = ScreenToRaster.Inverse;
            RasterToCamera = CameraToScreen.Inverse * RasterToScreen;
        }
    }
}
