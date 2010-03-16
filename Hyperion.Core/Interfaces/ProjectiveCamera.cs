
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
            CameraToScreen = projection;
        }
    }
}
