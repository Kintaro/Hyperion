
using System;
using Hyperion.Core;
using Hyperion.Core.Parallel;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;

namespace Hyperion.Renderers.Sampler
{
    public class SamplerRendererTask : ITask
    {
        private Scene Scene;
        private IRenderer Renderer;
        private ICamera Camera;
        private ISampler MainSampler;
        private Sample OrigSample;
        private int TaskNumber;
        private int TaskCount;

        public SamplerRendererTask (Scene scene, IRenderer renderer, ICamera camera, ISampler sampler, Sample sample, int tn, int tc)
        {
            Scene = scene;
            Camera = camera;
            Renderer = renderer;
            MainSampler = sampler;
            OrigSample = sample;
            TaskNumber = tn;
            TaskCount = tc;
        }

        public void Run ()
        {
            ISampler sampler = MainSampler.GetSubSampler (TaskNumber, TaskCount);
            if (sampler == null)
            {
                return;
            }

            int maxSamples = sampler.MaximumSampleCount;
            Sample[] samples = OrigSample.Duplicate (maxSamples);
            RayDifferential[] rays = new RayDifferential[maxSamples];
            Spectrum[] Ls = new Spectrum[maxSamples];
            Spectrum[] Ts = new Spectrum[maxSamples];
            Intersection[] isects = new Intersection[maxSamples];

            for (int i = 0; i < maxSamples; ++i)
            {
                samples[i] = new Sample ();
                rays[i] = new RayDifferential ();
                Ls[i] = new Spectrum ();
                Ts[i] = new Spectrum ();
                isects[i] = new Intersection ();
            }

            int sampleCount;

            while ((sampleCount = sampler.GetMoreSamples (samples)) > 0)
            {
                for (int i = 0; i < sampleCount; ++i)
                {
                    double rayWeight = Camera.GenerateRayDifferential (samples[i], ref rays[i]);
                    rays[i].ScaleDifferentials (1.0 / Math.Sqrt (sampler.SamplesPerPixel));

                    if (rayWeight > 0.0)
                        Ls[i] = rayWeight * Renderer.Li (Scene, rays[i], samples[i], ref isects[i], ref Ts[i]);
                    else
                    {
                        Ls[i] = new Spectrum ();
                        Ts[i] = new Spectrum (1.0);
                    }

                    // WARNINGS
                }

                if (sampler.ReportResults (samples, rays, Ls, isects, sampleCount))
                {
                    for (int i = 0; i < sampleCount; ++i)
                        Camera.Film.AddSample (samples[i], Ls[i]);
                }
            }

            Camera.Film.UpdateDisplay (sampler.xPixelStart, sampler.yPixelStart, sampler.xPixelEnd + 1, sampler.yPixelEnd + 1);
        }
    }
}
