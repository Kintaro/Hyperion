
using System;
using System.Threading;

namespace Hyperion.Core.Tools
{
    /// <summary>
    ///
    /// </summary>
    public class BlockedArray<T> where T : new()
    {
        /// <summary>
        ///
        /// </summary>
        private T[] data;

        /// <summary>
        ///
        /// </summary>
        private int uRes;

        /// <summary>
        ///
        /// </summary>
        private int vRes;

        /// <summary>
        ///
        /// </summary>
        private int uBlocks;

        /// <summary>
        ///
        /// </summary>
        private int logBlockSize;

        /// <summary>
        ///
        /// </summary>
        /// <param name="nu">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="nv">
        /// A <see cref="System.Int32"/>
        /// </param>
        public BlockedArray (int nu, int nv) : this(nu, nv, null, 2)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nu">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="nv">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="T[]"/>
        /// </param>
        public BlockedArray (int nu, int nv, T[] data) : this(nu, nv, data, 2)
        {
        }

        public static void Initialize (ref T[] data, int start, int count)
        {
            for (int i = 0; i < count; ++i)
                data[start + i] = new T ();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nu">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="nv">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="T[]"/>
        /// </param>
        /// <param name="logBlockSize">
        /// A <see cref="System.Int32"/>
        /// </param>
        public BlockedArray (int nu, int nv, T[] d, int logBlockSize)
        {
            this.logBlockSize = logBlockSize;
            uRes = nu;
            vRes = nv;
            uBlocks = RoundUp (uRes) >> logBlockSize;
            int nAlloc = RoundUp (uRes) * RoundUp (vRes);
            data = new T[nAlloc];
            
            Thread lower = new Thread (delegate() { Initialize (ref data, 0, data.Length / 2); });
            Thread upper = new Thread (delegate() { Initialize (ref data, data.Length / 2, data.Length / 2); });
            lower.Start ();
            upper.Start ();
            
            while (lower.IsAlive || upper.IsAlive)
                ;
            
            if (d == null)
                return;
            
            for (int v = 0; v < nv; ++v)
                for (int u = 0; u < nu; ++u)
                    Set (u, v, d[v * uRes + u]);
        }

        /// <summary>
        ///
        /// </summary>
        public int BlockSize {
            get { return 1 << logBlockSize; }
        }

        /// <summary>
        ///
        /// </summary>
        public int uSize {
            get { return uRes; }
        }

        /// <summary>
        ///
        /// </summary>
        public int vSize {
            get { return vRes; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int RoundUp (int x)
        {
            return (x + BlockSize - 1) & ~(BlockSize - 1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int Block (int x)
        {
            return x >> logBlockSize;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int Offset (int x)
        {
            return (x & (BlockSize - 1));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="u">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="v">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        public T Get (int u, int v)
        {
            int bu = Block (u), bv = Block (v);
            int ou = Offset (u), ov = Offset (v);
            int offset = BlockSize * BlockSize * (uBlocks * bv + bu);
            offset += BlockSize * ov + ou;
            
            return data[offset];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="u">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="v">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="d">
        /// A <see cref="T"/>
        /// </param>
        public void Set (int u, int v, T d)
        {
            int bu = Block (u), bv = Block (v);
            int ou = Offset (u), ov = Offset (v);
            int offset = BlockSize * BlockSize * (uBlocks * bv + bu);
            offset += BlockSize * ov + ou;
            
            data[offset] = d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="a">
        /// A <see cref="T[]"/>
        /// </param>
        public void GetLinearArray (T[] a)
        {
            int i = 0;
            for (int v = 0; v < vRes; ++v)
                for (int u = 0; u < uRes; ++u)
                    a[i++] = Get (u, v);
        }
    }
}
