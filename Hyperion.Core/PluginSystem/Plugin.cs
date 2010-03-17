
using System;
using System.IO;
using System.Reflection;

namespace Hyperion.Core.PluginSystem
{
    /// <summary>
    ///     This is the basic Plugin class. So far, a plugin holds track
    ///     of it's name and a library handle. To use a plugin, the pluginclass
    ///     has to inherit from Plugin and provide a Creation Method. Afterwards it
    ///     can be used like this:
    ///
    /// <code>
    ///     ICamera cam = PluginManager.CreateCamera ("PerspectiveCamera");
    /// </code>
    ///
    ///     Therefore your class must contain this (again, the camera as an example):
    ///
    /// <code>
    ///     public class CameraPlugin : Plugin
    ///     {
    ///         delegate ICamera CreateCameraDelegate ();
    ///         public CreateCameraDelegate CreateCamera;
    ///     }
    /// </code>
    /// </summary>
    public class Plugin
    {
        /// <summary>
        ///     The plugin's name
        /// </summary>
        private string _name;

        /// <summary>
        ///     The plugin's assembly
        /// </summary>
        private Assembly _assembly;

        /// <summary>
        ///     Initialize the plugin and it's name
        /// </summary>
        /// <param name="name">
        ///     The plugin's name
        /// </param>
        public Plugin (string category, string name)
        {
            category = category.Trim ();
            name = name.Trim ();
            _name = name;
            
            string currentDirectory = Directory.GetCurrentDirectory ();
            string[] files = Directory.GetFiles (currentDirectory);
            string assemblyPath = string.Empty;

            foreach (string file in files)
            {
                if (file.Contains (category + "." + name + ".dll"))
                {
                    assemblyPath = file;
                    break;
                }
            }
            
            _assembly = Assembly.LoadFile (assemblyPath);
        }

        /// <summary>
        ///     Retrieves a pointer to a method in the plugin's assembly
        /// </summary>
        /// <param name="methodName">
        ///     The method's name
        /// </param>
        /// <returns>
        ///     The method info
        /// </returns>
        public MethodInfo GetMethod (string methodName)
        {
            Type[] types = null;
            try
            {
                types = _assembly.GetTypes ();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            foreach (Type type in types)
            {
                if (type.Name == _name)
                    return type.GetMethod (methodName);
            }

            return null;
        }
    }
}
