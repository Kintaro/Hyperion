
using System;
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Accelerators.KdTree
{
    /// <summary>
    ///     The KdTreeAccelerator aggregate speeds up intersectiontesting
    ///     substantially by py putting a big bounding box
    ///     around the whole scene and then cutting it down into
    ///     several smaller boxes recursively, so that intersection
    ///     testing is sped up pretty fast.
    /// </summary>
    public class KdTree : IAggregate
    {
        /// <summary>
        ///     Cost for an intersection
        /// </summary>
        private int _isectCost;
        /// <summary>
        ///     Costs for traversing along an edge
        /// </summary>
        private int _traversalCost;
        /// <summary>
        ///     Maximum amount of primitives per leaf
        /// </summary>
        private int _maxPrimitives;
        /// <summary>
        ///
        /// </summary>
        private int _nextFreeNode;
        /// <summary>
        ///
        /// </summary>
        private int _numberOfAllocatedNodes;
        /// <summary>
        ///     Bonus for empty leafs
        /// </summary>
        private double _emptyBonus;
        /// <summary>
        ///
        /// </summary>
        private KdAcceleratorNode[] _nodes;
        /// <summary>
        ///
        /// </summary>
        private BoundingBox _bounds = new BoundingBox ();
        /// <summary>
        ///
        /// </summary>
        private List<IPrimitive> _primitives = new List<IPrimitive> ();
        /// <summary>
        ///
        /// </summary>
        private int _maxDepth;
        /// <summary>
        ///
        /// </summary>
        public static int NumberOfTotalNodes;
        /// <summary>
        ///
        /// </summary>
        public static int NumberOfTotalLeafs;
        /// <summary>
        ///
        /// </summary>
        internal ArrayPool<KdToDo> ToDoPool = new ArrayPool<KdToDo> (64);
        internal BoundEdgeComparer comparer = new BoundEdgeComparer ();

        /// <summary>
        ///
        /// </summary>
        internal struct KdToDo
        {
            /// <summary>
            ///
            /// </summary>
            public double tMin;
            /// <summary>
            ///
            /// </summary>
            public double tMax;
            /// <summary>
            ///
            /// </summary>
            public int Index;
        }

        /// <summary>
        ///     Creates a new KdTree
        /// </summary>
        /// <param name="p">
        ///     List of primitives to build the kd-Tree from
        /// </param>
        /// <param name="isectCost">
        ///     Cost for an intersection
        /// </param>
        /// <param name="traversalCost">
        ///     Cost for traversing along edges
        /// </param>
        /// <param name="emptyBonus">
        ///     Bonus for empty leafs
        /// </param>
        /// <param name="maxPrimitives">
        ///     Maximum amount of primitives per leaf
        /// </param>
        /// <param name="maxDepth">
        ///     Maximum recursion depth
        /// </param>
        public KdTree (List<IPrimitive> p, int isectCost, int traversalCost, double emptyBonus, int maxPrimitives, int maxDepth)
        {
            _isectCost = isectCost;
            _traversalCost = traversalCost;
            _emptyBonus = emptyBonus;
            _maxPrimitives = maxPrimitives;
            _maxDepth = maxDepth;

            Console.WriteLine ("    - Refining {0} primitives...", p.Count);
            foreach (IPrimitive primitive in p)
                primitive.FullyRefine (ref _primitives);
            Console.WriteLine ("    - Refined into {0} primitives", _primitives.Count);
            
            // Build kd-Tree
            _nextFreeNode = _numberOfAllocatedNodes = 0;
            if (_maxDepth <= 0)
                _maxDepth = Util.RoundToInt (8 + 1.3 * Util.Log2Int ((double)_primitives.Count));
            
            // Compute bounds for kd-Tree generation
            List<BoundingBox> primitiveBounds = new List<BoundingBox> ();
            primitiveBounds.Capacity = _primitives.Count;

            foreach (IPrimitive primitive in _primitives)
            {
                BoundingBox b = primitive.WorldBound;
                _bounds = BoundingBox.Union (_bounds, b);
                primitiveBounds.Add (b);
            }
            Console.WriteLine ("    - KdTree Volume: {0}", _bounds.Volume);
            
            // Allocate working memory for kd-Tree construction
            List<BoundEdge>[] edges = new List<BoundEdge>[3];
            edges[0] = new List<BoundEdge> ();
            edges[1] = new List<BoundEdge> ();
            edges[2] = new List<BoundEdge> ();
            
            for (int i = 0; i < 3; ++i)
            {
                edges[i].Capacity = 2 * _primitives.Count;
                for (int j = 0; j < 2 * _primitives.Count; ++j)
                    edges[i].Add (null);
            }
            
            int[] prims0 = new int[_primitives.Count];
            int[] prims1 = new int[(_maxDepth + 1) * _primitives.Count];
            
            // Initialize primNums for kd-Tree construction
            int[] primNums = new int[_primitives.Count];
            
            for (int i = 0; i < _primitives.Count; ++i)
                primNums[i] = i;
            
            // Start recursive construction of kd-Tree
            Console.WriteLine ("    - Building KdTree for {0} primitives...", _primitives.Count);
            BuildTree (0, _bounds, primitiveBounds, primNums, _primitives.Count, _maxDepth, edges, prims0, prims1, 0, 0);
            Console.WriteLine ("    - Created KdTree with {0} nodes and {1} leafs.", NumberOfTotalNodes, NumberOfTotalLeafs);
        }

        /// <summary>
        ///     Builds the Kd-Tree
        /// </summary>
        /// <param name="nodeNum">
        ///     The current node index
        /// </param>
        /// <param name="nodeBounds">
        ///     The node's bounding box
        /// </param>
        /// <param name="allPrimBounds">
        ///     The primitive's bounding boxes
        /// </param>
        /// <param name="primNums">
        ///     Array of primitive indices
        /// </param>
        /// <param name="numberOfPrimitives">
        ///     Number of primitives to build from
        /// </param>
        /// <param name="depth">
        ///     The current recursion depth
        /// </param>
        /// <param name="edges">
        ///     Array of edges
        /// </param>
        /// <param name="prims0">
        ///
        /// </param>
        /// <param name="prims1">
        ///
        /// </param>
        /// <param name="badRefines">
        ///     Amount of bad refines
        /// </param>
        public void BuildTree (int nodeNum, BoundingBox nodeBounds, List<BoundingBox> allPrimBounds,
            int[] primNums, int numberOfPrimitives, int depth, List<BoundEdge>[] edges,
            int[] prims0, int[] prims1, int primIndex,
        int badRefines)
        {
            if (_nextFreeNode == _numberOfAllocatedNodes)
            {
                int nAlloc = Math.Max (2 * _numberOfAllocatedNodes, 512);
                KdAcceleratorNode[] n = new KdAcceleratorNode[nAlloc];
                
                if (_numberOfAllocatedNodes > 0)
                {
                    _nodes.CopyTo (n, 0);
                }
                _nodes = n;
                _numberOfAllocatedNodes = nAlloc;
            }
            ++_nextFreeNode;

            // Initialize leaf nodes if termination criteria is met
            if (numberOfPrimitives <= _maxPrimitives || depth == 0)
            {
                _nodes[nodeNum].InitLeaf (primNums, numberOfPrimitives);
                return;
            }
            
            // Initialize interior node and continue recursion.
            // Choose split axis position for interior node
            int bestAxis = -1, bestOffset = -1;
            double bestCost = double.PositiveInfinity;
            double oldCost = _isectCost * numberOfPrimitives;
            double totalSA = nodeBounds.SurfaceArea;
            double invTotalSA = 1.0 / totalSA;
            Vector d = nodeBounds.pMax - nodeBounds.pMin;

            int axis = nodeBounds.MaximumExtent;
            int retries = 0;
            
            while (true)
            {
                // Initialize edges for axis
                for (int i = 0; i < numberOfPrimitives; ++i)
                {
                    int pn = (int)primNums[i];
                    BoundingBox box = allPrimBounds[pn];
                    edges[axis][2 * i] = new BoundEdge (box.pMin[axis], pn, true);
                    edges[axis][2 * i + 1] = new BoundEdge (box.pMax[axis], pn, false);
                }

                edges[axis].Sort (0, 2 * numberOfPrimitives, comparer);
                
                // Compute cost of all splits for axis to find best
                int nBelow = 0, nAbove = numberOfPrimitives;
                
                for (int i = 0; i < 2 * numberOfPrimitives; ++i)
                {
                    if (edges[axis][i].Type == BoundEdge.EdgeType.End)
                        --nAbove;
                    double edget = edges[axis][i].t;

                    if (edget > nodeBounds.pMin[axis] && edget < nodeBounds.pMax[axis])
                    {
                        int otherAxis0 = (axis + 1) % 3, otherAxis1 = (axis + 2) % 3;
                        
                        double belowSA = 2 * (d[otherAxis0] * d[otherAxis1] + (edget - nodeBounds.pMin[axis]) * (d[otherAxis0] + d[otherAxis1]));
                        double aboveSA = 2 * (d[otherAxis0] * d[otherAxis1] + (nodeBounds.pMax[axis] - edget) * (d[otherAxis0] + d[otherAxis1]));
                        double pBelow = belowSA * invTotalSA;
                        double pAbove = aboveSA * invTotalSA;
                        double eb = (nAbove == 0 || nBelow == 0) ? _emptyBonus : 0.0;
                        double cost = _traversalCost + _isectCost * (1.0 - eb) * (pBelow * nBelow + pAbove * nAbove);
                        
                        // Update best split if this is the lowest so far
                        if (cost < bestCost)
                        {
                            bestCost = cost;
                            bestAxis = axis;
                            bestOffset = i;
                        }
                    }
                    
                    if (edges[axis][i].Type == BoundEdge.EdgeType.Start)
                        ++nBelow;
                }
                
                // Create leaf if no good splits were found
                if (bestAxis == -1 && retries < 2)
                {
                    ++retries;
                    axis = (axis + 1) % 3;
                    continue;
                }
                break;
            }
            
            if (bestCost > oldCost)
                ++badRefines;
            
            if ((bestCost > 4.0 * oldCost && numberOfPrimitives < 16) || bestAxis == -1 || badRefines == 3)
            {
                _nodes[nodeNum].InitLeaf (primNums, numberOfPrimitives);
                return;
            }
            
            // Classify primitives with respect to split
            int n0 = 0, n1 = 0;
            for (int i = 0; i < bestOffset; ++i)
                if (edges[bestAxis][i].Type == BoundEdge.EdgeType.Start)
                    prims0[n0++] = edges[bestAxis][i].PrimitiveNumber;
            for (int i = bestOffset + 1; i < 2 * numberOfPrimitives; ++i)
                if (edges[bestAxis][i].Type == BoundEdge.EdgeType.End)
                    prims1[primIndex + n1++] = edges[bestAxis][i].PrimitiveNumber;
            
            // Recursively initialize child nodes
            double tsplit = edges[bestAxis][bestOffset].t;
            BoundingBox bounds0 = new BoundingBox (nodeBounds), bounds1 = new BoundingBox(nodeBounds);
            bounds0.pMax[bestAxis] = bounds1.pMin[bestAxis] = tsplit;
            
            BuildTree (nodeNum + 1, bounds0, allPrimBounds, prims0, n0, depth - 1, edges, prims0, prims1, numberOfPrimitives + primIndex, badRefines);
            int aboveChild = _nextFreeNode;
            _nodes[nodeNum].InitInterior (bestAxis, aboveChild, tsplit);
            BuildTree (aboveChild, bounds1, allPrimBounds, prims1, n1, depth - 1, edges, prims0, prims1, numberOfPrimitives + primIndex, badRefines);
        }

        /// <summary>
        ///     Returns true if the shape is capable of doing
        ///     intersection tests on it's own.
        ///     Otherwise Refine will be called later to create
        ///     a vector of intersection-capable shapes
        /// </summary>
        public override bool CanIntersect
        {
            get { return true; }
        }

        /// <summary>
        ///     Computes the object's bounding box in world space.
        /// </summary>
        public override BoundingBox WorldBound
        {
            get { return _bounds; }
        }

        /// <summary>
        ///     Checks if the given ray intersects the primitive.
        /// </summary>
        /// <param name="ray">
        ///     The ray to test for
        /// </param>
        /// <param name="intersection">
        ///     A pointer to an Intersection structure that'll hold
        ///     the intersection information.
        /// </param>
        /// <returns>
        ///     True if the ray intersects the primitive. False otherwise.
        /// </returns>
        public override bool Intersect (Ray ray, ref Intersection intersection)
        {
            int index = 0;

            double tMin = 0.0, tMax = 0.0;

            if (!_bounds.IntersectP (ray, out tMin, out tMax))
            {
                return false;
            }

            Vector inverseDirection = new Vector (1.0 / ray.Direction.x, 1.0 / ray.Direction.y, 1.0 / ray.Direction.z);

            int todoPosition = 0;
            bool hit = false;

            KdToDo[] todo = ToDoPool.Get ();
            
            while (index >= 0 && index < _nodes.Length)
            {
                if (ray.MaxT < tMin)
                    break;
                
                if (!_nodes[index].IsLeaf)
                {
                    int axis = _nodes[index].SplitAxis;
                    double tPlane = (_nodes[index].Split - ray.Origin[axis]) * inverseDirection[axis];
                    int firstIndex, secondIndex;
                    bool belowFirst = (ray.Origin[axis] < _nodes[index].Split) || (ray.Origin[axis] == _nodes[index].Split && ray.Direction[axis] < 0);
                    
                    if (belowFirst)
                    {
                        firstIndex = index + 1;
                        secondIndex = _nodes[index].AboveChild;
                    }
                    else
                    {
                        firstIndex = _nodes[index].AboveChild;
                        secondIndex = index + 1;
                    }

                    if (tPlane > tMax || tPlane <= 0)
                    {
                        index = firstIndex;
                    }
                    else if (tPlane < tMin)
                    {
                        index = secondIndex;
                    }
                    else
                    {
                        todo[todoPosition].tMax = tMax;
                        todo[todoPosition].tMin = tPlane;
                        todo[todoPosition].Index = secondIndex;
                        ++todoPosition;
                        index = firstIndex;
                        tMax = tPlane;
                    }
                }
                else
                {
                    if (_nodes[index].NumberOfPrimitives == 1)
                    {
                        IPrimitive prim = _primitives[_nodes[index].OnePrimitive];
                        if (prim.Intersect (ray, ref intersection))
                        {
                            hit = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _nodes[index].NumberOfPrimitives; ++i)
                        {
                            IPrimitive prim = _primitives[_nodes[index].Primitives[i]];
                            if (prim.Intersect (ray, ref intersection))
                            {
                                hit = true;
                            }
                        }
                    }
                    
                    if (todoPosition > 0)
                    {
                        --todoPosition;
                        tMin = todo[todoPosition].tMin;
                        tMax = todo[todoPosition].tMax;
                        index = todo[todoPosition].Index;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            ToDoPool.Free (todo);

            return hit;
        }

        /// <summary>
        ///     A quick check just to test if the ray intersects the primitive.
        ///     This method doesn't fill in Intersection information,
        ///     so this method is obviously faster for pure testing.
        /// </summary>
        /// <param name="ray">
        ///     The ray to test for
        /// </param>
        /// <returns>
        ///     True if the ray intersects the primitive. False otherwise.
        /// </returns>
        public override bool IntersectP (Ray ray)
        {
            int index = 0;

            double tMin = 0.0, tMax = 0.0;
            if (!_bounds.IntersectP (ray, out tMin, out tMax))
                return false;

            Vector inverseDirection = new Vector (1.0 / ray.Direction.x, 1.0 / ray.Direction.y, 1.0 / ray.Direction.z);
            
            KdToDo[] todo = ToDoPool.Get ();
            int todoPosition = 0;
            
            while (index >= 0 && index < _nodes.Length)
            {
                if (_nodes[index].IsLeaf)
                {
                    int numberOfPrimitives = _nodes[index].NumberOfPrimitives;
                    if (numberOfPrimitives == 1)
                    {
                        IPrimitive primitive = _primitives[_nodes[index].OnePrimitive];
                        if (primitive.IntersectP (ray))
                        {
                            ToDoPool.Free (todo);
                            return true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < numberOfPrimitives; ++i)
                        {
                            IPrimitive primitive = _primitives[_nodes[index].Primitives[i]];
                            if (primitive.IntersectP (ray))
                            {
                                ToDoPool.Free (todo);
                                return true;
                            }
                        }
                    }
                    
                    // Grab next node to process from todo list
                    if (todoPosition > 0)
                    {
                        --todoPosition;
                        tMin = todo[todoPosition].tMin;
                        tMax = todo[todoPosition].tMax;
                        index = todo[todoPosition].Index;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // Process kd-Tree interior node
                    // Compute parametric distance along ray to split plane
                    int axis = _nodes[index].SplitAxis;
                    double tPlane = (_nodes[index].Split - ray.Origin[axis]) * inverseDirection[axis];
                    
                    // Get node children for ray
                    bool belowFirst = (ray.Origin[axis] < _nodes[index].Split) || (ray.Origin[axis] == _nodes[index].Split && ray.Direction[axis] < 0);
                    
                    int firstIndex, secondIndex;
                    if (belowFirst)
                    {
                        firstIndex = index + 1;
                        secondIndex = _nodes[index].AboveChild;
                    }
                    else
                    {
                        firstIndex = _nodes[index].AboveChild;
                        secondIndex = index + 1;
                    }
                    
                    // Advance to next child node, possibly enqueue other child
                    if (tPlane > tMax || tPlane <= 0)
                    {
                        index = firstIndex;
                    }

                    else if (tPlane < tMin)
                    {
                        index = secondIndex;
                    }
                    else
                    {
                        todo[todoPosition].tMin = tPlane;
                        todo[todoPosition].tMax = tMax;
                        todo[todoPosition].Index = secondIndex;
                        ++todoPosition;
                        tMax = tPlane;
                        index = firstIndex;
                    }
                }
            }
            ToDoPool.Free (todo);
            return false;
        }

        /// <summary>
        ///     Creates a new Kd-Tree
        /// </summary>
        /// <param name="primitives">
        ///     List of primitives to include in the Tree
        /// </param>
        /// <param name="parameterSet">
        ///     Parameters for the Kd-Tree
        /// </param>
        /// <returns>
        ///     The newly created Kd-Tree
        /// </returns>
        public static IAggregate CreateAccelerator (List<IPrimitive> primitives, ParameterSet parameterSet)
        {
            int isectCost = parameterSet.FindOneInt ("intersectcost", 80);
            int traversalCost = parameterSet.FindOneInt ("traversalcost", 1);
            double emptyBonus = parameterSet.FindOneDouble ("emptybonus", 0.5);
            int maxPrimitives = parameterSet.FindOneInt ("maxprims", 1);
            int maxDepth = parameterSet.FindOneInt ("maxdepth", -1);
            
            return new KdTree (primitives, isectCost, traversalCost, emptyBonus, maxPrimitives, maxDepth);
        }
    }
}
