
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
    public class LightPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate ILight CreateLightDelegate (Transform lightToWorld, ParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateLightDelegate CreateLight;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public LightPlugin (string name) : base("Lights", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateLight");
            CreateLight = Delegate.CreateDelegate (typeof(CreateLightDelegate), methodInfo) as CreateLightDelegate;
        }
    }
}
