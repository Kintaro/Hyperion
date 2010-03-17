
using System.Collections.Generic;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Core.PluginSystem
{
    public static class PluginManager
    {
        public static ICamera CreateCamera (string name, ParameterSet paramSet, AnimatedTransform worldToCamera, IFilm film)
        {
            CameraPlugin plugin = new CameraPlugin (name);
            return plugin.CreateCamera (paramSet, worldToCamera, film);
        }

        public static IFilm CreateFilm (string name, ParameterSet paramSet, IFilter filter)
        {
            FilmPlugin plugin = new FilmPlugin (name);
            return plugin.CreateFilm (paramSet, filter);
        }

        public static ISurfaceIntegrator CreateSurfaceIntegrator (string name, ParameterSet paramSet)
        {
            SurfaceIntegratorPlugin plugin = new SurfaceIntegratorPlugin (name);
            return plugin.CreateSurfaceIntegrator (paramSet);
        }

        public static IVolumeIntegrator CreateVolumeIntegrator (string name, ParameterSet paramSet)
        {
            VolumeIntegratorPlugin plugin = new VolumeIntegratorPlugin (name);
            return plugin.CreateVolumeIntegrator (paramSet);
        }

        public static ITexture<double> CreateDoubleTexture (string name, Transform textureToWorld, TextureParameterSet parameters)
        {
            TexturePlugin plugin = new TexturePlugin (name);
            return plugin.CreateDoubleTexture (textureToWorld, parameters);
        }

        public static ITexture<Spectrum> CreateSpectrumTexture (string name, Transform textureToWorld, TextureParameterSet parameters)
        {
            TexturePlugin plugin = new TexturePlugin (name);
            return plugin.CreateSpectrumTexture (textureToWorld, parameters);
        }

        public static IPrimitive CreateAccelerator (string name, List<IPrimitive> primitives, ParameterSet parameters)
        {
            AcceleratorPlugin plugin = new AcceleratorPlugin (name);
            return plugin.CreateAccelerator (primitives, parameters);
        }

        public static IRenderer CreateRenderer (string name, ISampler sampler, ICamera camera, ISurfaceIntegrator surfaceIntegrator, IVolumeIntegrator volumeIntegrator)
        {
            RendererPlugin plugin = new RendererPlugin (name);
            return plugin.CreateRenderer (sampler, camera, surfaceIntegrator, volumeIntegrator);
        }

        public static IShape CreateShape (string name, Transform objectToWorld, bool reverseOrientation, ParameterSet parameters)
        {
            ShapePlugin plugin = new ShapePlugin (name);
            return plugin.CreateShape (objectToWorld, reverseOrientation, parameters);
        }

        public static AreaLight CreateAreaLight (string name, Transform objectToWorld, ParameterSet parameters, IShape shape)
        {
            if (name == "AreaLight")
            {
                Spectrum L = parameters.FindOneSpectrum ("L", new Spectrum (1.0));
                int numberOfSamples = parameters.FindOneInt ("nsamples", 1);
                //return new AreaLight (objectToWorld, L, numberOfSamples, shape);
                return null;
            }
            AreaLightPlugin plugin = new AreaLightPlugin (name);
            return plugin.CreateAreaLight (objectToWorld, parameters, shape) as AreaLight;
        }

        public static IMaterial CreateMaterial (string name, Transform objectToWorld, TextureParameterSet parameters)
        {
            MaterialPlugin plugin = new MaterialPlugin (name);
            return plugin.CreateMaterial (objectToWorld, parameters);
        }

        public static IFilter CreateFilter (string name, ParameterSet parameters)
        {
            FilterPlugin plugin = new FilterPlugin (name);
            IFilter filter = plugin.CreateFilter (parameters);
            return filter;
        }

        public static ISampler CreateSampler (string name, ParameterSet parameters, IFilm film, ICamera camera)
        {
            SamplerPlugin plugin = new SamplerPlugin (name);
            return plugin.CreateSampler (parameters, film, camera);
        }

        public static ILight CreateLight (string name, Transform lightToWorld, ParameterSet parameters)
        {
            LightPlugin plugin = new LightPlugin (name);
            return plugin.CreateLight (lightToWorld, parameters);
        }
    }
}