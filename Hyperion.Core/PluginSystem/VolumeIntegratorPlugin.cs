
using System;
using System.Reflection;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     Plugin for all kinds of volume integrators.
    /// </summary>
    public class VolumeIntegratorPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate IVolumeIntegrator CreateVolumeIntegratorDelegate (ParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateVolumeIntegratorDelegate CreateVolumeIntegrator;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public VolumeIntegratorPlugin (string name) : base("Integrators", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateVolumeIntegrator");
            CreateVolumeIntegrator = Delegate.CreateDelegate (typeof(CreateVolumeIntegratorDelegate), methodInfo) as CreateVolumeIntegratorDelegate;
        }
    }
}
