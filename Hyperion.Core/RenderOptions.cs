
using System.Collections.Generic;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;
using Hyperion.Core.PluginSystem;

namespace Hyperion.Core
{
    public sealed class RenderOptions
    {
        public string AcceleratorName;
        public ParameterSet AcceleratorParameters;
        public string CameraName;
        public ParameterSet CameraParameters;
        public string FilterName;
        public ParameterSet FilterParameters;
        public string FilmName;
        public ParameterSet FilmParameters;
        public string SamplerName;
        public ParameterSet SamplerParameters;
        public string SurfaceIntegratorName;
        public ParameterSet SurfaceIntegratorParameters;
        public string VolumeIntegratorName;
        public ParameterSet VolumeIntegratorParameters;
        public AnimatedTransform WorldToCamera;
        public List<IPrimitive> Primitives = new List<IPrimitive> ();
        public List<IVolumeRegion> VolumeRegions = new List<IVolumeRegion> ();
        public List<ILight> Lights = new List<ILight> ();
        public List<IPrimitive> CurrentInstance = null;

        public RenderOptions ()
        {
            FilmName = "Image";
            SamplerName = "Bestcandidate";
            AcceleratorName = "Grid";
            SurfaceIntegratorName = "Whitted";
            VolumeIntegratorName = "Emission";
            CameraName = "Perspective";
            FilterName = "Mitchell";
            SurfaceIntegratorParameters = new ParameterSet ();
            VolumeIntegratorParameters = new ParameterSet ();
            AcceleratorParameters = new ParameterSet ();
            FilterParameters = new ParameterSet ();
            FilmParameters = new ParameterSet ();
            SamplerParameters = new ParameterSet ();
        }

        public Scene CreateScene ()
        {
            IPrimitive accelerator = PluginManager.CreateAccelerator (AcceleratorName, Primitives, AcceleratorParameters);
            return new Scene (accelerator, Lights, null);
        }

        public IRenderer CreateRenderer ()
        {
            IFilter filter = PluginManager.CreateFilter (FilterName, FilterParameters);
            IFilm film = PluginManager.CreateFilm (FilmName, FilmParameters, filter);
            ICamera camera = PluginManager.CreateCamera (CameraName, CameraParameters, WorldToCamera, film);
            ISurfaceIntegrator surfaceIntegrator = PluginManager.CreateSurfaceIntegrator (SurfaceIntegratorName, SurfaceIntegratorParameters);
            IVolumeIntegrator volumeIntegrator = PluginManager.CreateVolumeIntegrator (VolumeIntegratorName, VolumeIntegratorParameters);
            ISampler sampler = PluginManager.CreateSampler (SamplerName, SamplerParameters, film);

            return PluginManager.CreateRenderer ("Sampler", sampler, camera, surfaceIntegrator, volumeIntegrator);
        }
    }
}
