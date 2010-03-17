
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Parallel;

namespace Hyperion.Renderers.Sampler
{
    public class Sampler : IRenderer
    {
        private ISampler MainSampler;
        private ICamera Camera;
        private ISurfaceIntegrator SurfaceIntegrator;
        private IVolumeIntegrator VolumeIntegrator;

        public Sampler (ISampler sampler, ICamera camera, ISurfaceIntegrator surfaceIntegrator, IVolumeIntegrator volumeIntegrator)
        {
            MainSampler = sampler;
            Camera = camera;
            SurfaceIntegrator = surfaceIntegrator;
            VolumeIntegrator = volumeIntegrator;
        }

        public override Spectrum Li (Scene scene, RayDifferential ray, Sample sample, ref Intersection isect, ref Spectrum T)
        {
            Spectrum Lo = new Spectrum ();

            if (scene.Intersect (ray, ref isect))
                Lo = SurfaceIntegrator.Li (scene, this, ray, isect, sample);
            else
                foreach (ILight light in scene.Lights)
                    Lo += light.Le (ray);

            Spectrum Lv = VolumeIntegrator.Li (scene, this, ray, sample, ref T);

            return T * Lo + Lv;
        }

        public override Spectrum Transmittance (Scene scene, RayDifferential ray, Sample sample)
        {
            return VolumeIntegrator.Transmittance (scene, this, ray, sample);
        }

        public override void Render (Scene scene)
        {
            SurfaceIntegrator.Preprocess (scene, Camera, this);
            VolumeIntegrator.Preprocess (scene, Camera, this);
            Sample sample = new Sample (MainSampler, SurfaceIntegrator, VolumeIntegrator, scene);

            int nPixels = Camera.Film.xResolution * Camera.Film.yResolution;
            int nTasks = Math.Max (32 * ParallelUtility.NumberOfSystemCores, nPixels / (16 * 16));
            nTasks = Util.RoundUpPow2 (nTasks);
            List<ITask> renderTasks = new List<ITask> ();

            for (int i = 0; i < nTasks; ++i)
                renderTasks.Add (new SamplerRendererTask (scene, this, Camera, MainSampler, sample, nTasks - 1 - i, nTasks));

            Console.WriteLine ("Enqueueing Tasks...");
            ParallelUtility.EnqueueTasks (renderTasks);
            Console.WriteLine ("Waiting for tasks to finish...");
            ParallelUtility.WaitForAllTasks ();
            renderTasks.Clear ();

            Camera.Film.WriteImage ();
        }

        public static IRenderer CreateRenderer (ISampler sampler, ICamera camera, ISurfaceIntegrator surfaceIntegrator, IVolumeIntegrator volumeIntegrator)
        {
            return new Sampler (sampler, camera, surfaceIntegrator, volumeIntegrator);
        }
    }
}
