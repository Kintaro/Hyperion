
using System.Collections.Generic;
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;
using Hyperion.Core.Tools;

namespace Hyperion.Shapes.TriangleMesh
{
    public class TriangleMesh : IShape
    {
        public int NumberOfTriangles;
        public int NumberOfVertices;
        public int[] VertexIndices;
        public Point[] Points;
        public Normal[] Normals;
        public Vector[] Vectors;
        public double[] Uvs;
        public ITexture<double> AlphaTexture;

        public TriangleMesh (Transform objectToWorld, Transform worldToObject, bool ro, int ntris, int nverts, int[] vptr, Point[] p, Normal[] n, Vector[] s, double[] uv,
        ITexture<double> atex) : base(objectToWorld, worldToObject, ro)
        {
            NumberOfTriangles = ntris;
            NumberOfVertices = nverts;
            AlphaTexture = atex;

            /*VertexIndices = new int[3 * NumberOfTriangles];
            vptr.CopyTo (VertexIndices, 0);*/
            VertexIndices = vptr;

            /*if (uv != null)
            {
                Uvs = new double[2 * NumberOfVertices];
                uv.CopyTo (Uvs, 0);
            }
            else
                Uvs = null;*/

            Points = new Point[NumberOfVertices];
            Normals = n;
            Vectors = s;
            Uvs = uv;

            /*if (n != null)
            {
                Normals = new Normal[NumberOfVertices];
                n.CopyTo (Normals, 0);
            }
            else
                Normals = null;

            if (s != null)
            {
                Vectors = new Vector[NumberOfVertices];
                s.CopyTo (Vectors, 0);
            }
            else
                Vectors = null;*/
            
            for (int i = 0; i < NumberOfVertices; ++i)
                Points[i] = objectToWorld.Apply (p[i]);
            
        }

        public override void Refine (ref List<IShape> refined)
        {
            for (int i = 0; i < NumberOfTriangles; ++i)
                refined.Add (new Triangle (ObjectToWorld, WorldToObject, ReverseOrientation, this, i));
        }


        public override BoundingBox ObjectBound {
            get {
                BoundingBox objectBounds = new BoundingBox ();
                for (int i = 0; i < NumberOfVertices; ++i)
                    objectBounds = BoundingBox.Union (objectBounds, WorldToObject.Apply (Points[i]));
                return objectBounds;
            }
        }

        public override BoundingBox WorldBound {
            get {
                BoundingBox worldBounds = new BoundingBox ();
                for (int i = 0; i < NumberOfVertices; ++i)
                    worldBounds = BoundingBox.Union (worldBounds, Points[i]);
                return worldBounds;
            }
        }

        public override bool CanIntersect {
            get { return false; }
        }

        public static IShape CreateShape (Transform o2w, Transform w2o, bool reverseOrientation, ParameterSet parameters, Dictionary<string, ITexture<double>> floatTextures)
        {
            int nvi = 0, npi = 0, nuvi = 0, nsi = 0, nni = 0;
            int[] vi = parameters.FindInt ("indices", ref nvi);
            Point[] P = parameters.FindPoint ("P", ref npi);
            double[] uvs = parameters.FindDouble ("uv", ref nuvi);
            if (uvs == null)
                uvs = parameters.FindDouble ("st", ref nuvi);
            bool discardDegnerateUVs = parameters.FindOneBool ("discarddegenerateUVs", false);
            // XXX should complain if uvs aren't an array of 2...
            if (uvs != null)
            {
                if (nuvi < 2 * npi)
                {
                    uvs = null;
                }
            }
            if (vi == null || P == null)
                return null;
            Vector[] S = parameters.FindVector ("S", ref nsi);
            if (S != null && nsi != npi)
            {
                
                S = null;
            }
            Normal[] N = parameters.FindNormal ("N", ref nni);
            if (N != null && nni != npi)
            {
                N = null;
            }
            if (discardDegnerateUVs && uvs != null && N != null)
            {
                // if there are normals, check for bad uv's that
                // give degenerate mappings; discard them if so
                int vp = 0;
                for (int i = 0; i < nvi; i += 3,vp += 3)
                {
                    double area = 0.5 * ((P[vi[vp + 0]] - P[vi[vp + 1]]) % (P[vi[vp + 2]] - P[vi[vp + 1]])).Length;
                    if (area < 1E-07)
                        continue;
                    // ignore degenerate tris.
                    if ((uvs[2 * vi[vp + 0]] == uvs[2 * vi[vp + 1]] && uvs[2 * vi[vp + 0] + 1] == uvs[2 * vi[vp + 1] + 1]) || (uvs[2 * vi[vp + 1]] == uvs[2 * vi[vp + 2]] && uvs[2 * vi[vp + 1] + 1] == uvs[2 * vi[vp + 2] + 1]) || (uvs[2 * vi[vp + 2]] == uvs[2 * vi[vp + 0]] && uvs[2 * vi[vp + 2] + 1] == uvs[2 * vi[vp + 0] + 1]))
                    {
                        uvs = null;
                        break;
                    }
                }
            }
            for (int i = 0; i < nvi; ++i)
                if (vi[i] >= npi)
                {
                    return null;
                }
            
            ITexture<double> alphaTex = null;
            string alphaTexName = parameters.FindTexture ("alpha");
            if (alphaTexName != "")
            {
                if (floatTextures.ContainsKey (alphaTexName))
                    alphaTex = floatTextures[alphaTexName];
            } else if (parameters.FindOneDouble ("alpha", 1.0) == 0.0)
                alphaTex = new ConstantTexture<double> (0.0);
            return new TriangleMesh (o2w, w2o, reverseOrientation, nvi / 3, npi, vi, P, N, S, uvs,
            alphaTex);
        }
    }
}
