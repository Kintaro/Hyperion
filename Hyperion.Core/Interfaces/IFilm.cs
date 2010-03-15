
using System;

namespace Hyperion.Core.Interfaces
{
    public abstract class IFilm
    {
        public readonly int xResolution;
        public readonly int yResolution;

        public IFilm (int xres, int yres)
        {
            xResolution = xres;
            yResolution = yres;
        }

        public abstract void AddSample (CameraSample sample, Spectrum L);
        public abstract void Splat (CameraSample sample, Spectrum L);
        public abstract void GetSampleExtent (out int xstart, out int xend, out int ystart, out int yend);
        public abstract void GetPixelExtent (out int xstart, out int xend, out int ystart, out int yend);
        public abstract void WriteImage (double splatScale);

        public virtual void WriteImage ()
        {
            WriteImage (1.0);
        }

        public virtual void UpdateDisplay (int x0, int y0, int x1, int y1)
        {
            UpdateDisplay (x0, y0, x1, y1, 1.0);
        }

        public virtual void UpdateDisplay (int x0, int y0, int x1, int y1, double splatScale)
        {
        }
    }
}
