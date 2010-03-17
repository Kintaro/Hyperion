
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
    public class ShapePlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate IShape CreateShapeDelegate (Transform objectToWorld, bool reverse, ParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateShapeDelegate CreateShape;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public ShapePlugin (string name) : base("Shapes", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateShape");
            CreateShape = Delegate.CreateDelegate (typeof(CreateShapeDelegate), methodInfo) as CreateShapeDelegate;
        }
    }
}
