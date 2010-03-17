
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
    public class TexturePlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate ITexture<double> CreateDoubleTextureDelegate (Transform worldToTexture, TextureParameterSet paramSet);
        /// <summary>
        ///
        /// </summary>
        public delegate ITexture<Spectrum> CreateSpectrumTextureDelegate (Transform worldToTexture, TextureParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateDoubleTextureDelegate CreateDoubleTexture;

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
        public TexturePlugin (string name) : base("Textures", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateTexture");
            CreateDoubleTexture = Delegate.CreateDelegate (typeof(CreateDoubleTextureDelegate), methodInfo) as CreateDoubleTextureDelegate;
            CreateSpectrumTexture = Delegate.CreateDelegate (typeof(CreateSpectrumTextureDelegate), methodInfo) as CreateSpectrumTextureDelegate;
        }
    }
}
