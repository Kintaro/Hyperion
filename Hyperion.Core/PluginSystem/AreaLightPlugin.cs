
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
    public class AreaLightPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate ILight CreateAreaLightDelegate (Transform objectToWorld, ParameterSet parameters, IShape shape);

        /// <summary>
        ///
        /// </summary>
        public CreateAreaLightDelegate CreateAreaLight;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public AreaLightPlugin (string name) : base("Lights", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateAreaLight");
            CreateAreaLight = Delegate.CreateDelegate (typeof(CreateAreaLightDelegate), methodInfo) as CreateAreaLightDelegate;
        }
    }
}
