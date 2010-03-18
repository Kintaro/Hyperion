
using System;
using System.Reflection;
using System.Collections.Generic;
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
        public delegate IShape CreateShapeDelegate (Transform objectToWorld, Transform worldToObject, bool reverse, ParameterSet paramSet, Dictionary<string, ITexture<double>> floatTextures);

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
