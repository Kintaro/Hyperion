
using System;
using System.Reflection;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     Plugin for all kinds of textures.
    /// </summary>
    public class SpectrumTexturePlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate ITexture<Spectrum> CreateSpectrumTextureDelegate (Transform worldToTexture, TextureParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateSpectrumTextureDelegate CreateSpectrumTexture;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public SpectrumTexturePlugin (string name) : base("Textures", name)
        {
            Console.WriteLine ("Trying to load " + name);
            MethodInfo methodInfo = GetMethod ("CreateSpectrumTexture");
            CreateSpectrumTexture = Delegate.CreateDelegate (typeof(CreateSpectrumTextureDelegate), methodInfo) as CreateSpectrumTextureDelegate;
        }
    }
}
