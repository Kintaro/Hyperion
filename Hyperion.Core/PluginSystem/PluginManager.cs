
using System.Collections.Generic;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Core.PluginSystem
{
    public static class PluginManager
    {
        public static ICamera CreateCamera (string name, ParameterSet paramSet, TransformSet camToWorldSet, double transformStart, double transformEnd, IFilm film)
        {
            Transform[] camToWorld = new Transform[2];
            Transform temp;
            Api.TransformCache.Lookup (camToWorldSet[0], out camToWorld[0], out temp);
            Api.TransformCache.Lookup (camToWorldSet[1], out camToWorld[1], out temp);
            AnimatedTransform animatedCamToWorld = new AnimatedTransform (camToWorld[0], transformStart, camToWorld[1], transformEnd);
            CameraPlugin plugin = new CameraPlugin (name);
            return plugin.CreateCamera (paramSet, animatedCamToWorld, film);
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
            /*TexturePlugin plugin = new TexturePlugin (name);
            return plugin.CreateDoubleTexture (textureToWorld, parameters);*/
            return null;
        }

        public static ITexture<Spectrum> CreateSpectrumTexture (string name, Transform textureToWorld, TextureParameterSet parameters)
        {
            /*SpectrumTexturePlugin plugin = new SpectrumTexturePlugin (name);
            return plugin.CreateSpectrumTexture (textureToWorld, parameters);*/
            return Hyperion.Core.Textures.Imagemap<Spectrum, Spectrum>.CreateSpectrumTexture (textureToWorld, parameters);
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

        public static IShape CreateShape (string name, Transform objectToWorld, Transform worldToObject, bool reverseOrientation, ParameterSet parameters, Dictionary<string, ITexture<double>> floatTextures, Dictionary<string, ITexture<Spectrum>> spectrumTextures)
        {
            ShapePlugin plugin = new ShapePlugin (name);
            return plugin.CreateShape (objectToWorld, worldToObject, reverseOrientation, parameters, floatTextures, spectrumTextures);
        }

        public static AreaLight CreateAreaLight (string name, Transform objectToWorld, ParameterSet parameters, IShape shape)
        {
            AreaLightPlugin plugin = new AreaLightPlugin (name);
            return plugin.CreateAreaLight (objectToWorld, parameters, shape);
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
