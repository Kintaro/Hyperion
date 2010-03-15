
using System;

namespace Hyperion.Core.Interfaces
{
    public class IIntegrator
    {
        public virtual void Preprocess (Scene scene, ICamera camera, IRenderer renderer)
        {
        }

        public virtual void RequestSamples (ISampler sampler, Sample sample, Scene scene)
        {

        }
    }
}
