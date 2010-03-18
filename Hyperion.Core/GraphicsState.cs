
using System.Collections.Generic;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;

namespace Hyperion.Core
{
    public class GraphicsState
    {
        public ParameterSet MaterialParameters = new ParameterSet ();
        public string Material;
        public ParameterSet AreaLightParameters = new ParameterSet ();
        public string AreaLight = "";
        public bool ReverseOrientation = false;
        public Dictionary<string, ITexture<double>> FloatTextures = new Dictionary<string, ITexture<double>> ();
        public Dictionary<string, ITexture<Spectrum>> SpectrumTextures = new Dictionary<string, ITexture<Spectrum>> ();
        public string CurrentNamedMaterial = "";
        public Dictionary<string, IMaterial> NamedMaterials = new Dictionary<string, IMaterial> ();

        public GraphicsState ()
        {
            MaterialParameters = new ParameterSet ();
            AreaLightParameters = new ParameterSet ();
            Material = "Matte";
            ReverseOrientation = false;
        }

        public IMaterial CreateMaterial (ParameterSet parameters)
        {
            TextureParameterSet mp = new TextureParameterSet (parameters, MaterialParameters, FloatTextures, SpectrumTextures);
            IMaterial material = null;

            if (CurrentNamedMaterial != "" && NamedMaterials.ContainsKey (CurrentNamedMaterial))
            {
                material = NamedMaterials[Api.GraphicsState.CurrentNamedMaterial];
            }
            if (material == null)
                material = PluginSystem.PluginManager.CreateMaterial (Material, Api.CurrentTransform[0], mp);
            if (material == null)
                material = PluginSystem.PluginManager.CreateMaterial ("Matte", Api.CurrentTransform[0], mp);
            return material;
        }
    }
}
