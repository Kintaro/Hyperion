
using System;

namespace Hyperion.Core.Tools
{
    /// <summary>
    ///     Represents a single parameter and is used in ParamSet.
    /// </summary>
    public struct ParameterSetItem<T>
    {
        /// <summary>
        ///
        /// </summary>
        public T[] Data;

        /// <summary>
        ///
        /// </summary>
        public string Name;

        /// <summary>
        ///
        /// </summary>
        public int NumberOfItems;

        /// <summary>
        ///
        /// </summary>
        public bool LookedUp;

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="values">
        /// A <see cref="T[]"/>
        /// </param>
        public ParameterSetItem (string name, T[] values)
        {
            Name = name;
            NumberOfItems = values.Length;
            Data = new T[NumberOfItems];
            
            for (int i = 0; i < NumberOfItems; ++i)
            {
                Data[i] = values[i];
            }
            LookedUp = false;
        }
    }
}
