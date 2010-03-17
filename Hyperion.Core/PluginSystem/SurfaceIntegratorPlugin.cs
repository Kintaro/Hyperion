
using System;
using System.Reflection;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     Plugin for all kinds of surface integrators.
    /// </summary>
    public class SurfaceIntegratorPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate ISurfaceIntegrator CreateSurfaceIntegratorDelegate (ParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateSurfaceIntegratorDelegate CreateSurfaceIntegrator;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public SurfaceIntegratorPlugin (string name) : base("Integrators", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateSurfaceIntegrator");
            CreateSurfaceIntegrator = Delegate.CreateDelegate (typeof(CreateSurfaceIntegratorDelegate), methodInfo) as CreateSurfaceIntegratorDelegate;
        }
    }
}
