
using System;

namespace Hyperion.Core.Interfaces
{
    public class IPrimitive
    {
        private static int NextPrimitiveID;
        public readonly int PrimitiveID;

        public IPrimitive ()
        {
            PrimitiveID = NextPrimitiveID++;
        }

        public virtual bool CanIntersect {
            get { return true; }
        }
    }
}
