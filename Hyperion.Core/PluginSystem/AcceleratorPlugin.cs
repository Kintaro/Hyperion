
using System;
using System.Collections.Generic;
using System.Reflection;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     Plugin for all kinds of cameras.
    /// </summary>
    public class AcceleratorPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate IPrimitive CreateAcceleratorDelegate (List<IPrimitive> primitives, ParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateAcceleratorDelegate CreateAccelerator;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public AcceleratorPlugin (string name) : base("Accelerators", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateAccelerator");
            CreateAccelerator = Delegate.CreateDelegate (typeof(CreateAcceleratorDelegate), methodInfo) as CreateAcceleratorDelegate;
        }
    }
}
