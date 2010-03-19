
using System;
using Hyperion.Core;
using Hyperion.Core.Geometry;
using Hyperion.Core.Reflection;

namespace Hyperion.Core.Interfaces
{
    public static class IntegratorBase
    {
        public static Spectrum SpecularReflect (RayDifferential ray, BSDF bsdf, Intersection isect, IRenderer renderer, Scene scene, Sample sample)
        {
            Vector wo = -ray.Direction, wi = new Vector ();
            double pdf = 0.0;
            Point p = bsdf.dgShading.p;
            Normal n = bsdf.dgShading.n;

            Spectrum f = bsdf.SampleF (wo, ref wi, new BSDFSample (), ref pdf, BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR);
            Spectrum L = new Spectrum ();

            if (pdf > 0.0 && !f.IsBlack && Util.AbsDot (wi, n) != 0.0)
            {
                RayDifferential rd = new RayDifferential (p, wi, ray, isect.RayEpsilon);
                if (ray.HasDifferentials)
                {
                    rd.HasDifferentials = true;
                    rd.RxOrigin = p + isect.dg.dpdx;
                    rd.RyOrigin = p + isect.dg.dpdy;

                    Normal dndx = bsdf.dgShading.dndu * bsdf.dgShading.dudx + bsdf.dgShading.dndv * bsdf.dgShading.dvdx;
                    Normal dndy = bsdf.dgShading.dndu * bsdf.dgShading.dudy + bsdf.dgShading.dndv * bsdf.dgShading.dvdy;
                    Vector dwodx = -ray.RxDirection - wo, dwody = -ray.Direction - wo;
                    double dDNdx = (dwodx ^ n) + (wo ^ dndx);
                    double dDNdy = (dwody ^ n) + (wo ^ dndy);
                    rd.RxDirection = wi - dwodx + 2 * new Vector ((wo ^ n) * dndx + dDNdx * n);
                    rd.RyDirection = wi - dwody + 2 * new Vector ((wo ^ n) * dndy + dDNdy * n);
                }

                Spectrum Li = renderer.Li (scene, rd, sample);
                L = f * Li * Util.AbsDot (wi, n) / pdf;
            }

            return L;
        }

        public static Spectrum SpecularTransmit (RayDifferential ray, BSDF bsdf, Intersection isect, IRenderer renderer, Scene scene, Sample sample)
        {
            Vector wo = -ray.Direction, wi = new Vector ();
            double pdf = 0.0;
            Point p = bsdf.dgShading.p;
            Normal n = bsdf.dgShading.n;
            Spectrum f = bsdf.SampleF (wo, ref wi, new BSDFSample (), ref pdf, BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
            Spectrum L = new Spectrum ();

            if (pdf > 0.0 && !f.IsBlack && Util.AbsDot (wi, n) != 0.0)
            {
                RayDifferential rd = new RayDifferential (p, wi, ray, isect.RayEpsilon);
                if (ray.HasDifferentials)
                {
                    rd.HasDifferentials = true;
                    rd.RxOrigin = p + isect.dg.dpdx;
                    rd.RyOrigin = p + isect.dg.dpdy;

                    double eta = bsdf.Eta;
                    Vector w = -wo;

                    if ((wo ^ n) < 0.0)
                        eta = 1.0 / eta;

                    Normal dndx = bsdf.dgShading.dndu * bsdf.dgShading.dudx + bsdf.dgShading.dndv * bsdf.dgShading.dvdx;
                    Normal dndy = bsdf.dgShading.dndu * bsdf.dgShading.dudy + bsdf.dgShading.dndv * bsdf.dgShading.dvdy;

                    Vector dwodx = -ray.RxDirection - wo, dwody = -ray.RyDirection - wo;
                    double dDNdx = (dwodx ^ n) + (wo ^ dndx);
                    double dDNdy = (dwody ^ n) + (wo ^ dndy);
                    double mu = eta * (w ^ n) - (wi ^ n);
                    double dmudx = (eta - (eta * eta * (w ^ n)) / (wi ^ n)) * dDNdx;
                    double dmudy = (eta - (eta * eta * (w ^ n)) / (wi ^ n)) * dDNdy;

                    rd.RxDirection = wi + eta * dwodx - new Vector (mu * dndx + dmudx * n);
                    rd.RyDirection = wi + eta * dwody - new Vector (mu * dndy + dmudy * n);
                }

                Spectrum Li = renderer.Li (scene, rd, sample);
                L = f * Li * Util.AbsDot (wi, n) / pdf;
            }

            return L;
        }
    }
}
