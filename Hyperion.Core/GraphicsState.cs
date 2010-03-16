
using System.Collections.Generic;
using Hyperion.Core.Geometry;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Tools;

namespace Hyperion.Core
{
    public class GraphicsState
    {
        public ParameterSet MaterialParameters;
        public string Material;
        public ParameterSet AreaLightParameters;
        public string AreaLight;
        public bool ReverseOrientation;

        public GraphicsState ()
        {
            MaterialParameters = new ParameterSet ();
            AreaLightParameters = new ParameterSet ();
            Material = "Matte";
            ReverseOrientation = false;
        }
    }
}
