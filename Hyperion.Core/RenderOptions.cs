
using System.Collections.Generic;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;

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
        public Transform WorldToCamera;
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
            SurfaceIntegratorParameters = new ParameterSet ();
            VolumeIntegratorParameters = new ParameterSet ();
            AcceleratorParameters = new ParameterSet ();
        }

        public Scene CreateScene ()
        {
            return null;
        }

        public IRenderer CreateRenderer ()
        {
            return null;
        }
    }
}
