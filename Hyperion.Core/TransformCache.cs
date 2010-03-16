
using System;
using System.Collections.Generic;
using Hyperion.Core.Geometry;

namespace Hyperion.Core
{
    public class TransformCache
    {
        private Dictionary<Transform, KeyValuePair<Transform, Transform>> Cache = new Dictionary<Transform, KeyValuePair<Transform, Transform>> ();

        public void Lookup (Transform t, out Transform cached, out Transform cachedInverse)
        {
            if (!Cache.ContainsKey (t))
            {
                Transform tr = new Transform (t);
                Transform tinv = new Transform (t.Inverse);
                Cache[t] = new KeyValuePair<Transform, Transform> (tr, tinv);
            }

            cached = Cache[t].Key;
            cachedInverse = Cache[t].Value;
        }

        public void Clear ()
        {
            Cache.Clear ();
        }
    }
}
