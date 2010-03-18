
using System;
using System.Collections.Generic;
using Hyperion.Core.Tools;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;

namespace Hyperion.Core
{
    public static class Api
    {
        public const string Version = "0.0.1";
        public static readonly DateTime VersionDate = new DateTime (2010, 3, 17, 20, 23, 50);
        public static TransformSet CurrentTransform = new TransformSet ();
        public static Dictionary<string, TransformSet> NamedCoordinateSystems = new Dictionary<string, TransformSet> ();
        public static RenderOptions RenderOptions = new RenderOptions ();
        public static GraphicsState GraphicsState = new GraphicsState ();
        public static Stack<GraphicsState> PushedGraphicsStates = new Stack<GraphicsState> ();
        public static Stack<TransformSet> PushedTransforms = new Stack<TransformSet> ();
        public static Stack<int> PushedActiveTransformBits = new Stack<int> ();
        public static TransformCache TransformCache = new TransformCache ();
        public static int ActiveTransformBits;

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
            Api.PushedGraphicsStates.Push (Api.GraphicsState);
            Api.PushedTransforms.Push (Api.CurrentTransform);
            Api.PushedActiveTransformBits.Push (Api.ActiveTransformBits);
        }

        public static void AttributeEnd ()
        {
            Api.GraphicsState = Api.PushedGraphicsStates.Pop ();
            Api.CurrentTransform = Api.PushedTransforms.Pop ();
            Api.ActiveTransformBits = Api.PushedActiveTransformBits.Pop ();
        }

        public static void Camera (string name, ParameterSet parameterSet)
        {
            Api.RenderOptions.CameraName = name;
            Api.RenderOptions.CameraParameters = parameterSet;
            Api.RenderOptions.CameraToWorld = CurrentTransform.Inverse;
            NamedCoordinateSystems["camera"] = Api.RenderOptions.CameraToWorld;
        }

        public static void ConcatTransform (double[] tr)
        {
            for (int i = 0; i < 2; ++i)
                if ((Api.ActiveTransformBits & (1 << i)) != 0)
                    Api.CurrentTransform[i] = Api.CurrentTransform[i] * new Transform (new Matrix (tr[0], tr[4], tr[8], tr[12],
                          tr[1], tr[5], tr[9], tr[13],
                          tr[2], tr[6], tr[10], tr[14],
                          tr[3], tr[7], tr[11], tr[15]));
        }

        public static void CoordSysTransform (string name)
        {
            if (Api.NamedCoordinateSystems.ContainsKey (name))
                Api.CurrentTransform = Api.NamedCoordinateSystems[name];
        }

        public static void Film (string name, ParameterSet parameterSet)
        {
            Api.RenderOptions.FilmName = name;
            Api.RenderOptions.FilmParameters = parameterSet;
        }

        public static void Light (string name, ParameterSet parameterSet)
        {
            ILight light = PluginSystem.PluginManager.CreateLight (name, CurrentTransform[0], parameterSet);
            Api.RenderOptions.Lights.Add (light);
        }

        public static void LookAt (double ex, double ey, double ez, double lx, double ly, double lz, double ux, double uy, double uz)
        {
            for (int i = 0; i < 2; ++i)
                if ((Api.ActiveTransformBits & (1 << i)) != 0)
                    Api.CurrentTransform[i] = Api.CurrentTransform[i] * Transform.LookAt (new Point (ex, ey, ez), new Point (lx, ly, lz), new Vector (ux, uy, uz));
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
            for (int i = 0; i < 2; ++i)
                if ((Api.ActiveTransformBits & (1 << i)) != 0)
                    Api.CurrentTransform[i] = Api.CurrentTransform[i] * Transform.Rotate (angle, new Vector (x, y, z));
        }

        public static void Sampler (string name, ParameterSet parameterSet)
        {
        }

        public static void Scale (double x, double y, double z)
        {
            for (int i = 0; i < 2; ++i)
                if ((Api.ActiveTransformBits & (1 << i)) != 0)
                    Api.CurrentTransform[i] = Api.CurrentTransform[i] * Transform.Scale (x, y, z);
        }

        public static void Shape (string name, ParameterSet parameterSet)
        {
            Console.WriteLine (" > Creating shape {0}", name);
            IPrimitive prim = null;
            AreaLight area = null;

            if (!CurrentTransform.IsAnimated)
            {
                Transform objectToWorld, worldToObject;
                TransformCache.Lookup (CurrentTransform[0], out objectToWorld, out worldToObject);
                IShape shape = PluginSystem.PluginManager.CreateShape (name, objectToWorld, worldToObject, Api.GraphicsState.ReverseOrientation, parameterSet, Api.GraphicsState.FloatTextures);
                if (shape == null)
                    return;
                IMaterial material = Api.GraphicsState.CreateMaterial (parameterSet);

                if (Api.GraphicsState.AreaLight != "")
                {
                    area = PluginSystem.PluginManager.CreateAreaLight (Api.GraphicsState.AreaLight, CurrentTransform[0], Api.GraphicsState.AreaLightParameters, shape);
                }
                prim = new GeometricPrimitive (shape, material, area);
            }

            if (Api.RenderOptions.CurrentInstance != null && area != null)
                Api.RenderOptions.CurrentInstance.Add (prim);
            else
            {
                Api.RenderOptions.Primitives.Add (prim);
                if (area != null)
                    Api.RenderOptions.Lights.Add (area);
            }


        }

        public static void SurfaceIntegrator (string name, ParameterSet parameterSet)
        {
        }

        public static void Texture (string name, string type, string texname, ParameterSet parameterSet)
        {
        }

        public static void TransformBegin ()
        {
            Api.PushedTransforms.Push (Api.CurrentTransform);
            Api.PushedActiveTransformBits.Push (Api.ActiveTransformBits);
        }

        public static void TransformEnd ()
        {
            Api.CurrentTransform = Api.PushedTransforms.Pop ();
            Api.ActiveTransformBits = Api.PushedActiveTransformBits.Pop ();
        }

        public static void Translate (double x, double y, double z)
        {
            for (int i = 0; i < 2; ++i)
                if ((Api.ActiveTransformBits & (1 << i)) != 0)
                    Api.CurrentTransform[i] = Api.CurrentTransform[i] * Transform.Translate (x, y, z);
        }

        public static void VolumeIntegrator (string name, ParameterSet parameterSet)
        {
        }

        public static void WorldBegin ()
        {
            CurrentTransform[0] = new Transform ();
            CurrentTransform[1] = new Transform ();
            ActiveTransformBits = 3;
            NamedCoordinateSystems["world"] = CurrentTransform;
        }

        public static void WorldEnd ()
        {
            IRenderer renderer = Api.RenderOptions.CreateRenderer ();
            Scene scene = Api.RenderOptions.CreateScene ();

            if (scene != null && renderer != null)
                renderer.Render (scene);
        }
    }
}
