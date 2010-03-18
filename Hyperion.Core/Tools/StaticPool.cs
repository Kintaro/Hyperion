
using System;
using System.Collections.Generic;

namespace Hyperion.Core.Tools
{
    /// <summary>
    ///
    /// </summary>
    public static class StaticPool<T> where T : new()
    {
        /// <summary>
        ///
        /// </summary>
        private static Stack<T> _items = new Stack<T> ();
        /// <summary>
        ///
        /// </summary>
        private static object _sync = new object ();

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        public static T Get ()
        {
            lock (_sync)
            {
                if (_items.Count == 0)
                {
                    return new T ();
                }
                else
                {
                    return _items.Pop ();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item">
        /// A <see cref="T"/>
        /// </param>
        public static void Free (T item)
        {
            lock (_sync)
            {
                _items.Push (item);
            }
        }
    }
}
