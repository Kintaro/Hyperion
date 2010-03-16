
using System;
using System.Collections.Generic;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;

namespace Hyperion.Core
{
    public sealed class Scene
    {
        public IPrimitive Aggregate;
        public List<ILight> Lights;
        public IVolumeRegion VolumeRegion;
        public BoundingBox Bound;

        public Scene (IPrimitive accelerator, List<ILight> lights, IVolumeRegion volumeRegion)
        {
            Lights = lights;
            Aggregate = accelerator;
            VolumeRegion = volumeRegion;

            Bound = Aggregate.WorldBound;

            if (VolumeRegion != null)
                Bound = BoundingBox.Union (Bound, VolumeRegion.WorldBound);
        }

        public bool Intersect (Ray ray, ref Intersection isect)
        {
            return Aggregate.Intersect (ray, ref isect);
        }

        public bool IntersectP (Ray ray)
        {
            return Aggregate.IntersectP (ray);
        }

        public BoundingBox WorldBound
        {
            get
            {
                return Bound;
            }
        }
    }
}
