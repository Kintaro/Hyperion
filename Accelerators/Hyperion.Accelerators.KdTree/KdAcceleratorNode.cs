
using System;
using System.Runtime.InteropServices;

namespace Hyperion.Accelerators.KdTree
{
    /// <summary>
    ///
    /// </summary>
    public struct KdAcceleratorNode
    {
        /// <summary>
        ///
        /// </summary>
        private int flags;

        /// <summary>
        ///
        /// </summary>
        public int OnePrimitive;

        /// <summary>
        ///
        /// </summary>
        public int[] Primitives;

        /// <summary>
        ///
        /// </summary>
        public double Split;

        /// <summary>
        ///
        /// </summary>
        public int NumberOfPrimitives;

        /// <summary>
        ///
        /// </summary>
        public int AboveChild;

        /// <summary>
        ///
        /// </summary>
        public bool IsLeaf
        {
            get { return (flags & 3) == 3; }
        }

        /// <summary>
        ///
        /// </summary>
        public int SplitAxis
        {
            get { return flags & 3; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="axis">
        /// A <see cref="System.UInt32"/>
        /// </param>
        /// <param name="ac">
        /// A <see cref="System.UInt32"/>
        /// </param>
        /// <param name="s">
        /// A <see cref="System.Single"/>
        /// </param>
        public void InitInterior (int axis, int ac, double s)
        {
            ++KdTree.NumberOfTotalNodes;
            Split = s;
            flags = axis;
            AboveChild = ac;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primNums">
        /// A <see cref="System.UInt32[]"/>
        /// </param>
        /// <param name="np">
        /// A <see cref="System.Int32"/>
        /// </param>
        public void InitLeaf (int[] primNums, int np)
        {
            ++KdTree.NumberOfTotalLeafs;
            NumberOfPrimitives = np;
            flags = 3;

            if (np == 0)
                OnePrimitive = 0;
            else if (np == 1)
                OnePrimitive = primNums[0];
            else
            {
                Primitives = new int[np];
                for (int i = 0; i < np; ++i)
                    Primitives[i] = primNums[i];
            }
        }
    }
}
