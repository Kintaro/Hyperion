
using System;
using System.Reflection;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     Plugin for all kinds of cameras.
    /// </summary>
    public class MaterialPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate IMaterial CreateMaterialDelegate (Transform objectToWorld, TextureParameterSet parameters);

        /// <summary>
        ///
        /// </summary>
        public CreateMaterialDelegate CreateMaterial;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public MaterialPlugin (string name) : base("Materials", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateMaterial");
            CreateMaterial = Delegate.CreateDelegate (typeof(CreateMaterialDelegate), methodInfo) as CreateMaterialDelegate;
        }
    }
}
