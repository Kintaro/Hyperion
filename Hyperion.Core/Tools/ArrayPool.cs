
using System;
using System.Collections.Generic;

namespace Hyperion.Core.Tools
{
    /// <summary>
    ///
    /// </summary>
    public class ArrayPool<T>
        where T : new()
    {
        /// <summary>
        ///
        /// </summary>
        private Stack<T[]> _items = new Stack<T[]> ();
        /// <summary>
        ///
        /// </summary>
        private int _size = 1;
        /// <summary>
        ///
        /// </summary>
        private object _sync = new object ();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">
        /// A <see cref="System.Int32"/>
        /// </param>
        public ArrayPool (int size)
        {
            _size = size;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        public T[] Get ()
        {
            lock (_sync)
            {
                if (_items.Count == 0)
                {
                    return new T [_size];
                } else
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
        public void Free (T[] item)
        {
            lock (_sync)
            {
                _items.Push (item);
            }
        }
    }
}
