
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
    public class FilterPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate IFilter CreateFilterDelegate (ParameterSet paramSet);

        /// <summary>
        ///
        /// </summary>
        public CreateFilterDelegate CreateFilter;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public FilterPlugin (string name) : base("Filters", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateFilter");
            CreateFilter = Delegate.CreateDelegate (typeof(CreateFilterDelegate), methodInfo) as CreateFilterDelegate;
        }
    }
}
