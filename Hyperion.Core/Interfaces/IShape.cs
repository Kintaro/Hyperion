
using System;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public class IShape
    {
        public static int NextShapeID;
        public readonly Transform ObjectToWorld;
        public readonly Transform WorldToObject;
        public readonly bool ReverseOrientation;
        public readonly bool TransformSwapsHandedness;
        public readonly int ShapeID;

        public IShape (Transform objectToWorld, Transform worldToObject, bool reverseOrientation)
        {
            ObjectToWorld = new Transform (objectToWorld);
            WorldToObject = new Transform (worldToObject);
            ReverseOrientation = reverseOrientation;
            TransformSwapsHandedness = ObjectToWorld.SwapsHandedness;
            ShapeID = NextShapeID++;
        }
    }
}
