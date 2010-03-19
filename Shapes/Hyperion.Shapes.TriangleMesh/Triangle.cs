
using Hyperion.Core;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;

namespace Hyperion.Shapes.TriangleMesh
{
    public class Triangle : IShape
    {
        private TriangleMesh Mesh;
        private int[] Vertices = new int[3];

        public Triangle (Transform objectToWorld, Transform worldToObject, bool ro, TriangleMesh mesh, int n) : base(objectToWorld, worldToObject, ro)
        {
            Mesh = mesh;
            Vertices[0] = mesh.VertexIndices[3 * n];
            Vertices[1] = mesh.VertexIndices[3 * n + 1];
            Vertices[2] = mesh.VertexIndices[3 * n + 2];
        }

        public override bool Intersect (Ray ray, ref double tHit, ref double rayEpsilon, ref DifferentialGeometry dg)
        {
            Point p1 = Mesh.Points[Vertices[0]];
            Point p2 = Mesh.Points[Vertices[1]];
            Point p3 = Mesh.Points[Vertices[2]];

            Vector e1 = p2 - p1;
            Vector e2 = p3 - p1;
            Vector s1 = ray.Direction % e2;

            double divisor = s1 ^ e1;
            if (divisor == 0.0)
                return false;
            double invDivisor = 1.0 / divisor;

            Vector d = ray.Origin - p1;
            double b1 = (d ^ s1) * invDivisor;
            if (b1 < 0.0 || b1 > 1.0)
                return false;

            Vector s2 = d % e1;
            double b2 = (ray.Direction ^ s2) * invDivisor;
            if (b2 < 0.0 || b1 + b2 > 1.0)
                return false;

            double t = (e2 ^ s2) * invDivisor;
            if (t < ray.MinT || t > ray.MaxT)
                return false;
            
            Vector dpdu = new Vector (), dpdv = new Vector ();
            double[] uvs = new double[6];
            GetUVs (uvs);
            
            double du1 = uvs[0] - uvs[4];
            double du2 = uvs[2] - uvs[4];
            double dv1 = uvs[1] - uvs[5];
            double dv2 = uvs[3] - uvs[5];
            Vector dp1 = p1 - p3, dp2 = p2 - p3;
            double determinant = du1 * dv2 - dv1 * du2;
            if (determinant == 0.0)
                Util.CoordinateSystem ((e2 % e1).Normalized, out dpdu, out dpdv);
            else
            {
                double invdet = 1.0 / determinant;
                dpdu = (dv2 * dp1 - dv1 * dp2) * invdet;
                dpdv = (-du2 * dp1 - du1 * dp2) * invdet;
            }
            
            double b0 = 1 - b1 - b2;
            double tu = b0 * uvs[0] + b1 * uvs[2] + b2 * uvs[4];
            double tv = b0 * uvs[1] + b1 * uvs[3] + b2 * uvs[5];
            
            if (ray.Depth != -1)
            {
                if (Mesh.AlphaTexture != null)
                {
                    DifferentialGeometry dgLocal = new DifferentialGeometry (ray.Apply (t), dpdu, dpdv, new Normal (), new Normal (), tu, tv, this);
                    if (Mesh.AlphaTexture.Evaluate (dgLocal) == 0.0)
                        return false;
                }
            }
            
            dg = new DifferentialGeometry (ray.Apply (t), dpdu, dpdv, new Normal (), new Normal (), tu, tv, this);
            tHit = t;
            rayEpsilon = 0.001 * tHit;
            return true;
        }

        public override bool IntersectP (Ray ray)
        {
            Point p1 = Mesh.Points[Vertices[0]];
            Point p2 = Mesh.Points[Vertices[1]];
            Point p3 = Mesh.Points[Vertices[2]];

            Vector e1 = p2 - p1;
            Vector e2 = p3 - p1;
            Vector s1 = ray.Direction % e2;
            
            double divisor = s1 ^ e1;
            if (divisor == 0.0)
                return false;
            double invDivisor = 1.0 / divisor;
            
            Vector d = ray.Origin - p1;
            double b1 = (d ^ s1) * invDivisor;
            if (b1 < 0.0 || b1 > 1.0)
                return false;
            
            Vector s2 = d % e1;
            double b2 = (ray.Direction ^ s2) * invDivisor;
            if (b2 < 0.0 || b1 + b2 > 1.0)
                return false;
            
            double t = (e2 ^ s2) * invDivisor;
            if (t < ray.MinT || t > ray.MaxT)
                return false;
            
            if (ray.Depth != -1)
            {
                Vector dpdu = new Vector (), dpdv = new Vector ();
                double[] uvs = new double[6];
                GetUVs (uvs);
                
                double du1 = uvs[0] - uvs[4];
                double du2 = uvs[2] - uvs[4];
                double dv1 = uvs[1] - uvs[5];
                double dv2 = uvs[3] - uvs[5];
                Vector dp1 = p1 - p3, dp2 = p2 - p3;
                double determinant = du1 * dv2 - dv1 * du2;
                if (determinant == 0.0)
                    Util.CoordinateSystem ((e2 % e1).Normalized, out dpdu, out dpdv);
                else
                {
                    double invdet = 1.0 / determinant;
                    dpdu = (dv2 * dp1 - dv1 * dp2) * invdet;
                    dpdv = (-du2 * dp1 - du1 * dp2) * invdet;
                }
                
                double b0 = 1 - b1 - b2;
                double tu = b0 * uvs[0] + b1 * uvs[2] + b2 * uvs[4];
                double tv = b0 * uvs[1] + b1 * uvs[3] + b2 * uvs[5];
                
                
                DifferentialGeometry dgLocal = new DifferentialGeometry (ray.Apply (t), dpdu, dpdv, new Normal (), new Normal (), tu, tv, this);
                if (Mesh.AlphaTexture != null && Mesh.AlphaTexture.Evaluate (dgLocal) == 0.0)
                    return false;
            }
            
            return true;
        }

        public override void GetShadingGeometry (Transform objectToWorld, DifferentialGeometry dg, ref DifferentialGeometry dgShading)
        {
            if (Mesh.Normals == null && Mesh.Vectors == null)
            {
                dgShading = new DifferentialGeometry (dg);
                return;
            }
            // Initialize _Triangle_ shading geometry with _n_ and _s_
            // Compute barycentric coordinates for point
            double[] b = new double[3];
            
            // Initialize _A_ and _C_ matrices for barycentrics
            double[] uv = new double[6];
            GetUVs (uv);
            double[][] A = new double[][] { new double[] { uv[2] - uv[0], uv[4] - uv[0] }, new double[] { uv[3] - uv[1], uv[5] - uv[1] } };
            double[] C = new double[] { dg.u - uv[0], dg.v - uv[1] };
            if (!Util.SolveLinearSystem2x2 (A, C[0], C[1], out b[1], out b[2]))
            {
                // Handle degenerate parametric mapping
                b[0] = b[1] = b[2] = 1.0 / 3.0;
            } else
                b[0] = 1.0 - b[1] - b[2];
            
            // Use _n_ and _s_ to compute shading tangents for triangle, _ss_ and _ts_
            Normal ns;
            Vector ss, ts;
            if (Mesh.Normals != null)
                ns = objectToWorld.Apply (b[0] * Mesh.Normals[Vertices[0]] +
                    b[1] * Mesh.Normals[Vertices[1]] +
                    b[2] * Mesh.Normals[Vertices[2]]).Normalized;
            else
                ns = new Normal (dg.n);
            if (Mesh.Vectors != null)
                ss = objectToWorld.Apply (b[0] * Mesh.Vectors[Vertices[0]] +
                    b[1] * Mesh.Vectors[Vertices[1]] +
                    b[2] * Mesh.Vectors[Vertices[2]]).Normalized;
            else
                ss = dg.dpdu.Normalized;
            ts = ss % ns;
            if (ts.SquaredLength > 0.0)
            {
                ts = ts.Normalized;
                ss = ts % ns;
            } else
                Util.CoordinateSystem (new Vector (ns), out ss, out ts);
            Normal dndu, dndv;
            
            // Compute $\dndu$ and $\dndv$ for triangle shading geometry
            if (Mesh.Normals != null)
            {
                double[] uvs = new double[6];
                GetUVs (uvs);
                // Compute deltas for triangle partial derivatives of normal
                double du1 = uvs[0] - uvs[4];
                double du2 = uvs[2] - uvs[4];
                double dv1 = uvs[1] - uvs[5];
                double dv2 = uvs[3] - uvs[5];
                Normal dn1 = Mesh.Normals[Vertices[0]] - Mesh.Normals[Vertices[2]];
                Normal dn2 = Mesh.Normals[Vertices[1]] - Mesh.Normals[Vertices[2]];
                double determinant = du1 * dv2 - dv1 * du2;
                if (determinant == 0.0)
                {
                    dndu = new Normal (0, 0, 0);
                    dndv = new Normal (0, 0, 0);
                } else
                {
                    double invdet = 1.0 / determinant;
                    dndu = (dv2 * dn1 - dv1 * dn2) * invdet;
                    dndv = (-du2 * dn1 + du1 * dn2) * invdet;
                }
            } else
            {
                dndu = new Normal (0, 0, 0);
                dndv = new Normal (0, 0, 0);
            }
            dgShading = new DifferentialGeometry (dg.p, ss, ts, ObjectToWorld.Apply (dndu), ObjectToWorld.Apply (dndv), dg.u, dg.v, dg.Shape);
            dgShading.dudx = dg.dudx;
            dgShading.dvdx = dg.dvdx;
            dgShading.dudy = dg.dudy;
            dgShading.dvdy = dg.dvdy;
            dgShading.dpdx = new Vector (dg.dpdx);
            dgShading.dpdy = new Vector (dg.dpdy);
        }

        public override Point Sample (double u1, double u2, ref Normal Ns)
        {
            Point p1 = Mesh.Points[Vertices[0]];
            Point p2 = Mesh.Points[Vertices[1]];
            Point p3 = Mesh.Points[Vertices[2]];

            double b1, b2;
            MonteCarlo.UniformSampleTriangle (u1, u2, out b1, out b2);
            
            Point p = b1 * p1 + b2 * p2 + (1.0 - b1 - b2) * p3;
            Normal n = new Normal ((p2 - p1) % (p3 - p1));
            Ns = n.Normalized;
            if (ReverseOrientation)
                Ns *= -1.0;
            return p;
        }

        public override double Area {
            get {
                Point p1 = Mesh.Points[Vertices[0]];
                Point p2 = Mesh.Points[Vertices[1]];
                Point p3 = Mesh.Points[Vertices[2]];

                return 0.5 * ((p2 - p1) % (p3 - p1)).Length;
            }
        }

        public override BoundingBox ObjectBound {
            get {
                Point p1 = Mesh.Points[Vertices[0]];
                Point p2 = Mesh.Points[Vertices[1]];
                Point p3 = Mesh.Points[Vertices[2]];

                return BoundingBox.Union (new BoundingBox (WorldToObject.Apply (p1), WorldToObject.Apply (p2)), WorldToObject.Apply (p3));
            }
        }

        public override BoundingBox WorldBound {
            get {
                Point p1 = Mesh.Points[Vertices[0]];
                Point p2 = Mesh.Points[Vertices[1]];
                Point p3 = Mesh.Points[Vertices[2]];

                return BoundingBox.Union (new BoundingBox (p1, p2), p3);
            }
        }

        private void GetUVs (double[] uv)
        {
            if (Mesh.Uvs != null)
            {
                uv[0] = Mesh.Uvs[2 * Vertices[0]];
                uv[1] = Mesh.Uvs[2 * Vertices[0] + 1];
                uv[2] = Mesh.Uvs[2 * Vertices[1]];
                uv[3] = Mesh.Uvs[2 * Vertices[1] + 1];
                uv[4] = Mesh.Uvs[2 * Vertices[2]];
                uv[5] = Mesh.Uvs[2 * Vertices[2] + 1];
            } else
            {
                uv[0] = 0.0;
                uv[1] = 0.0;
                uv[2] = 1.0;
                uv[3] = 0.0;
                uv[4] = 1.0;
                uv[5] = 1.0;
            }
        }
    }
}
