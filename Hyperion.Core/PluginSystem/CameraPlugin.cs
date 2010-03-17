
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
    public class CameraPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate ICamera CreateCameraDelegate (ParameterSet paramSet, Transform worldToCamera, IFilm film);

        /// <summary>
        ///
        /// </summary>
        public CreateCameraDelegate CreateCamera;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public CameraPlugin (string name) : base("Cameras", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateCamera");
            CreateCamera = Delegate.CreateDelegate (typeof(CreateCameraDelegate), methodInfo) as CreateCameraDelegate;
        }
    }
}
