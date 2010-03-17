
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
    public class FilmPlugin : Plugin
    {
        /// <summary>
        ///
        /// </summary>
        public delegate IFilm CreateFilmDelegate (ParameterSet paramSet, IFilter filter);

        /// <summary>
        ///
        /// </summary>
        public CreateFilmDelegate CreateFilm;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public FilmPlugin (string name) : base("Films", name)
        {
            MethodInfo methodInfo = GetMethod ("CreateFilm");
            CreateFilm = Delegate.CreateDelegate (typeof(CreateFilmDelegate), methodInfo) as CreateFilmDelegate;
        }
    }
}
