
using System;
using System.Collections.Generic;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;
using Hyperion.Core.PluginSystem;

namespace Hyperion.Core
{
    public sealed class RenderOptions
    {
        public double TransformStart = 0.0;
        public double TransformEnd = 1.0;
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
        public TransformSet CameraToWorld = null;
        public List<IPrimitive> Primitives = new List<IPrimitive> ();
        public List<IVolumeRegion> VolumeRegions = new List<IVolumeRegion> ();
        public List<ILight> Lights = new List<ILight> ();
        public List<IPrimitive> CurrentInstance = null;

        public RenderOptions ()
        {
            FilmName = "Image";
            SamplerName = "LowDiscrepancy";
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
            CameraParameters = new ParameterSet ();
        }

        public Scene CreateScene ()
        {
            Console.WriteLine ("  > Loading Accelerator Module");
            IPrimitive accelerator = PluginManager.CreateAccelerator (AcceleratorName, Primitives, AcceleratorParameters);
            return new Scene (accelerator, Lights, null);
        }

        public IRenderer CreateRenderer ()
        {
            Console.WriteLine ("  > Loading Filter Module");
            IFilter filter = PluginManager.CreateFilter (FilterName, FilterParameters);
            Console.WriteLine ("  > Loading Film Module");
            IFilm film = PluginManager.CreateFilm (FilmName, FilmParameters, filter);
            Console.WriteLine ("  > Loading Camera Module");
            ICamera camera = PluginManager.CreateCamera (CameraName, CameraParameters, CameraToWorld, Api.RenderOptions.TransformStart, Api.RenderOptions.TransformEnd, film);
            Console.WriteLine ("  > Loading Surface Integrator Module");
            ISurfaceIntegrator surfaceIntegrator = PluginManager.CreateSurfaceIntegrator (SurfaceIntegratorName, SurfaceIntegratorParameters);
            Console.WriteLine ("  > Loading Volume Integrator Module");
            IVolumeIntegrator volumeIntegrator = PluginManager.CreateVolumeIntegrator (VolumeIntegratorName, VolumeIntegratorParameters);
            Console.WriteLine ("  > Loading Sampler Module");
            ISampler sampler = PluginManager.CreateSampler (SamplerName, SamplerParameters, film, camera);

            Console.WriteLine ("  > Loading Renderer Module");
            return PluginManager.CreateRenderer ("Sampler", sampler, camera, surfaceIntegrator, volumeIntegrator);
        }
    }
}
