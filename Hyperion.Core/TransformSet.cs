
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core
{
    public class TransformSet
    {
        private Transform[] t = new Transform[] { new Transform (), new Transform () };

        public TransformSet ()
        {}

        public TransformSet (TransformSet tset)
        {
            t[0] = new Transform (tset[0]);
            t[1] = new Transform (tset[1]);
        }

        public Transform this[int index] {
            get { return t[index]; }
            set { t[index] = value; }
        }

        public TransformSet Inverse {
            get {
                TransformSet result = new TransformSet ();
                for (int i = 0; i < 2; ++i)
                    result.t[i] = t[i].Inverse;
                return result;
            }
        }

        public bool IsAnimated {
            get { return (t[0] != t[1]); }
        }
    }
}
