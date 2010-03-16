
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public struct VisibilityTester
    {
        public Ray Ray;

        public void SetSegment (Point p1, double eps1, Point p2, double eps2, double time)
        {
            double dist = Util.Distance (p1, p2);
            Ray = new Ray (p1, (p2 - p1) / dist, eps1, dist * (1.0 - eps2), time);
        }

        public void SetRay (Point p, double eps, Vector w, double time)
        {
            Ray = new Ray (p, w, eps, double.PositiveInfinity, time);
        }

        public Spectrum Transmittance (Scene scene, IRenderer renderer, Sample sample)
        {
            return renderer.Transmittance (scene, new RayDifferential (Ray), sample);
        }

        public bool Unoccluded (Scene scene)
        {
            return !scene.IntersectP (Ray);
        }
    }
}
