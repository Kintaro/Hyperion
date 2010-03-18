
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Accelerators.GridAccelerator
{
    public class Grid : IAggregate
    {
        private List<IPrimitive> Primitives = new List<IPrimitive> ();
        private int[] nVoxels = new int[3];
        private BoundingBox Bounds = new BoundingBox ();
        private Vector Width = new Vector ();
        private Vector InvWidth = new Vector ();
        private Voxel[] Voxels;

        public Grid (List<IPrimitive> primitives, bool refineImmediately)
        {
            if (refineImmediately)
            {
                foreach (IPrimitive primitive in primitives)
                {
                    primitive.FullyRefine (Primitives);
                }
            }
            else
                Primitives = primitives;

            foreach (IPrimitive primitive in Primitives)
                Bounds = BoundingBox.Union (Bounds, primitive.WorldBound);

            Vector delta = Bounds.pMax - Bounds.pMin;

            int maxAxis = Bounds.MaximumExtent;
            double invMaxWidth = 1.0 / delta[maxAxis];
            double cubeRoot = 3.0 * Math.Pow (Primitives.Count, 1.0 / 3.0);
            double voxelsPerUnitDistance = cubeRoot * invMaxWidth;

            for (int axis = 0; axis < 3; ++axis)
            {
                nVoxels[axis] = Util.RoundToInt (delta[axis] * voxelsPerUnitDistance);
                nVoxels[axis] = Util.Clamp (nVoxels[axis], 1, 64);
            }

            for (int axis = 0; axis < 3; ++axis)
            {
                Width[axis] = delta[axis] / nVoxels[axis];
                InvWidth[axis] = (Width[axis] == 0.0) ? 0.0 : 1.0 / Width[axis];
            }
            int nv = nVoxels[0] * nVoxels[1] * nVoxels[2];
            Voxels = new Voxel[nv];

            foreach (IPrimitive primitive in Primitives)
            {
                BoundingBox pb = primitive.WorldBound;
                int[] vmin = new int[3];
                int[] vmax = new int[3];

                for (int axis = 0; axis < 3; ++axis)
                {
                    vmin[axis] = PositionToVoxel (pb.pMin, axis);
                    vmax[axis] = PositionToVoxel (pb.pMax, axis);
                }

                for (int z = vmin[2]; z <= vmax[2]; ++z)
                {
                    for (int y = vmin[1]; y <= vmax[1]; ++y)
                    {
                        for (int x = vmin[0]; x <= vmax[0]; ++x)
                        {
                            int o = Offset (x, y, z);
                            if (Voxels[o] == null)
                            {
                                Voxels[o] = new Voxel (primitive);
                            }
                            else
                            {
                                Voxels[o].AddPrimitive (primitive);
                            }
                        }
                    }
                }
            }
        }

        public override BoundingBox WorldBound
        {
            get
            {
                return Bounds;
            }
        }

        public override bool Intersect (Ray ray, ref Intersection intersection)
        {
            double rayT = 0.0, t = 0.0;
            if (Bounds.Inside (ray.Apply (ray.MinT)))
                rayT = ray.MinT;
            else if (!Bounds.IntersectP (ray, out rayT, out t))
                return false;
            Console.WriteLine ("Still here, haha");
            Point gridIntersect = ray.Apply (rayT);

            double[] nextCrossing = new double[3];
            double[] delta = new double[3];
            int[] Step = new int[3];
            int[] Out = new int[3];
            int[] Pos = new int[3];

            for (int axis = 0; axis < 3; ++axis)
            {
                Pos[axis] = PositionToVoxel (gridIntersect, axis);
                if (ray.Direction[axis] >= 0)
                {
                    nextCrossing[axis] = rayT + (VoxelToPos (Pos[axis] + 1, axis) - gridIntersect[axis]) / ray.Direction[axis];
                    delta[axis] = Width[axis] / ray.Direction[axis];
                    Step[axis] = 1;
                    Out[axis] = nVoxels[axis];
                }
                else
                {
                    nextCrossing[axis] = rayT + (VoxelToPos(Pos[axis], axis) - gridIntersect[axis]) / ray.Direction[axis];
                    delta[axis] = -Width[axis] / ray.Direction[axis];
                    Step[axis] = -1;
                    Out[axis] = -1;
                }
            }

            bool hitSomething = false;
            while (true)
            {
                Voxel voxel = Voxels[Offset (Pos[0], Pos[1], Pos[2])];
                if (voxel != null)
                {
                    hitSomething |= voxel.Intersect (ray, ref intersection);
                }

                int bits = (((nextCrossing[0] < nextCrossing[1]) ? 1 : 0) << 2) + (((nextCrossing[0] < nextCrossing[2]) ? 1 : 0) << 1) + (((nextCrossing[1] < nextCrossing[2]) ? 1 : 0));
                int[] cmpTpAxis = new int[] { 2, 1, 2, 1, 2, 2, 0, 0 };
                int stepAxis = cmpTpAxis[bits];
                if (ray.MaxT < nextCrossing[stepAxis])
                {
                    break;
                }
                Pos[stepAxis] += Step[stepAxis];
                if (Pos[stepAxis] == Out[stepAxis])
                    break;
                nextCrossing[stepAxis] += delta[stepAxis];
            }
            return hitSomething;
        }

        public override bool IntersectP (Ray ray)
        {
            double rayT = 0.0, t = 0.0;
            if (Bounds.Inside (ray.Apply (ray.MinT)))
                rayT = ray.MinT;
            else if (!Bounds.IntersectP (ray, out rayT, out t))
                return false;
            Point gridIntersect = ray.Apply (rayT);
            
            double[] nextCrossing = new double[3];
            double[] delta = new double[3];
            int[] Step = new int[3];
            int[] Out = new int[3];
            int[] Pos = new int[3];
            
            for (int axis = 0; axis < 3; ++axis)
            {
                Pos[axis] = PositionToVoxel (gridIntersect, axis);
                if (ray.Direction[axis] >= 0)
                {
                    nextCrossing[axis] = rayT + (VoxelToPos (Pos[axis] + 1, axis) - gridIntersect[axis]) / ray.Direction[axis];
                    delta[axis] = Width[axis] / ray.Direction[axis];
                    Step[axis] = 1;
                    Out[axis] = nVoxels[axis];
                }

                else
                {
                    nextCrossing[axis] = rayT + (VoxelToPos (Pos[axis], axis) - gridIntersect[axis]) / ray.Direction[axis];
                    delta[axis] = -Width[axis] / ray.Direction[axis];
                    Step[axis] = -1;
                    Out[axis] = -1;
                }
            }

            while (true)
            {
                Voxel voxel = Voxels[Offset (Pos[0], Pos[1], Pos[2])];
                if (voxel != null)
                {
                    if (voxel.IntersectP (ray))
                        return true;
                }
                
                int bits = (((nextCrossing[0] < nextCrossing[1]) ? 1 : 0) << 2) + (((nextCrossing[0] < nextCrossing[2]) ? 1 : 0) << 1) + (((nextCrossing[1] < nextCrossing[2]) ? 1 : 0));
                int[] cmpTpAxis = new int[] { 2, 1, 2, 1, 2, 2, 0, 0 };
                int stepAxis = cmpTpAxis[bits];
                if (ray.MaxT < nextCrossing[stepAxis])
                {
                    break;
                }
                Pos[stepAxis] += Step[stepAxis];
                if (Pos[stepAxis] == Out[stepAxis])
                    break;
                nextCrossing[stepAxis] += delta[stepAxis];
            }
            return false;
        }

        private int PositionToVoxel (Point p, int axis)
        {
            int v = (int) ((p[axis] - Bounds.pMin[axis]) * InvWidth[axis]);
            return Util.Clamp (v, 0, nVoxels[axis] - 1);
        }

        private double VoxelToPos (int p, int axis)
        {
            return Bounds.pMin[axis] + p * Width[axis];
        }

        private int Offset (int x, int y, int z)
        {
            return z * nVoxels[0] * nVoxels[1] + y * nVoxels[0] + x;
        }

        public static IAggregate CreateAccelerator (List<IPrimitive> primitives, ParameterSet parameters)
        {
            bool refineImmediately = parameters.FindOneBool ("refineimmediately", false);
            return new Grid (primitives, refineImmediately);
        }
    }
}
