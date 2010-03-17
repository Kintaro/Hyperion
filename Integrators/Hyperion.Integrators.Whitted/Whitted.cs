
using System;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;

namespace Hyperion.Integrators.Whitted
{
    public class Whitted : ISurfaceIntegrator
    {
        private int MaxDepth;

        public Whitted (int md)
        {
            MaxDepth = md;
        }

        public override Spectrum Li (Scene scene, IRenderer renderer, RayDifferential ray, Intersection isect, Sample sample)
        {
            Spectrum L = new Spectrum ();
            BSDF bsdf = isect.GetBSDF (ray);

            Point p = bsdf.dgShading.p;
            Normal n = bsdf.dgShading.n;
            Vector wo = -ray.Direction;

            L += isect.Le (wo);

            foreach (ILight light in scene.Lights)
            {
                Vector wi = new Vector ();
                double pdf = 0.0;
                VisibilityTester visibility = new VisibilityTester ();

                Spectrum Li = light.SampleL (p, isect.RayEpsilon, new LightSample (), ray.Time, ref wi, ref pdf, ref visibility);
                if (Li.IsBlack || pdf == 0.0)
                    continue;

                Spectrum f = bsdf.F (wo, wi);
                if (!f.IsBlack && visibility.Unoccluded (scene))
                    L += f * Li * Util.AbsDot (wi, n) * visibility.Transmittance (scene, renderer, sample) / pdf;
            }

            if (ray.Depth + 1 < MaxDepth)
            {
                L += IntegratorBase.SpecularReflect (ray, bsdf, isect, renderer, scene, sample);
                L += IntegratorBase.SpecularTransmit (ray, bsdf, isect, renderer, scene, sample);
            }

            return L;
        }
    }
}
