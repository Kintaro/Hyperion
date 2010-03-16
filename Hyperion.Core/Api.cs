
using System;
using System.Collections.Generic;
using Hyperion.Core.Tools;

namespace Hyperion.Core
{
    public static class Api
    {
        public static RenderOptions RenderOptions = new RenderOptions ();
        public static List<GraphicsState> PushedGraphicsStates = new List<GraphicsState> ();

        public static void Accelerator (string name, ParameterSet parameterSet)
        {
            Api.RenderOptions.AcceleratorName = name;
            Api.RenderOptions.AcceleratorParameters = parameterSet;
        }

        public static void AreaLightSource (string name, ParameterSet parameterSet)
        {

        }

        public static void AttributeBegin ()
        {
        }

        public static void AttributeEnd ()
        {
        }

        public static void Camera (string name, ParameterSet parameterSet)
        {
        }

        public static void ConcatTransform (double[] numbers)
        {
        }

        public static void CoordSysTransform (string name)
        {
        }

        public static void Film (string name, ParameterSet parameterSet)
        {
        }

        public static void Light (string name, ParameterSet parameterSet)
        {
        }

        public static void LookAt (double ex, double ey, double ez, double lx, double ly,
                                   double lz, double ux, double uy, double uz)
        {
        }

        public static void Material (string name, ParameterSet parameterSet)
        {
        }

        public static void PixelFilter (string name, ParameterSet parameterSet)
        {
        }

        public static void ReverseOrientation ()
        {
        }

        public static void Rotate (double angle, double x, double y, double z)
        {
        }

        public static void Sampler (string name, ParameterSet parameterSet)
        {
        }

        public static void Scale (double x, double y, double z)
        {
        }

        public static void Shape (string name, ParameterSet parameterSet)
        {
        }

        public static void SurfaceIntegrator (string name, ParameterSet parameterSet)
        {
        }

        public static void Texture (string name, string type, string texname, ParameterSet parameterSet)
        {
        }

        public static void TransformBegin ()
        {
        }

        public static void TransformEnd ()
        {
        }

        public static void Translate (double x, double y, double z)
        {
        }

        public static void VolumeIntegrator (string name, ParameterSet parameterSet)
        {
        }

        public static void WorldBegin ()
        {
        }

        public static void WorldEnd ()
        {
        }
    }
}
