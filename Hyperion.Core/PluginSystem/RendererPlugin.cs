
using System;
using System.Collections.Generic;
using System.Reflection;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     Plugin for all kinds of renderers
    /// </summary>
    public class RendererPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate IRenderer CreateRendererDelegate (ISampler sampler, ICamera camera, ISurfaceIntegrator surfaceIntegrator, IVolumeIntegrator volumeIntegrator);

        /// <summary>
        ///
        /// </summary>
        public CreateRendererDelegate CreateRenderer;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public RendererPlugin (string name) : base("Renderers", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateRenderer");
            CreateRenderer = Delegate.CreateDelegate (typeof(CreateRendererDelegate), methodInfo) as CreateRendererDelegate;
        }
    }
}
