
using System;
using System.Reflection;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     Plugin for all kinds of films.
    /// </summary>
    public class SamplerPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate ISampler CreateSamplerDelegate (ParameterSet paramSet, IFilm film);

        /// <summary>
        ///
        /// </summary>
        public CreateSamplerDelegate CreateSampler;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public SamplerPlugin (string name) : base("Samplers", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateSampler");
            CreateSampler = Delegate.CreateDelegate (typeof(CreateSamplerDelegate), methodInfo) as CreateSamplerDelegate;
        }
    }
}
