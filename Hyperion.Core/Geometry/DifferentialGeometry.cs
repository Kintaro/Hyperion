
using System;
using Hyperion.Core.Interfaces;

namespace Hyperion.Core.Geometry
{
    public class DifferentialGeometry
    {
        public Point p;
        public Normal n;
        public double u;
        public double v;
        public Vector dpdu;
        public Vector dpdv;
        public Normal dndu;
        public Normal dndv;
        public Vector dpdx;
        public Vector dpdy;
        public double dudx;
        public double dudy;
        public double dvdx;
        public double dvdy;
        public readonly IShape Shape;

        public DifferentialGeometry ()
        {
            p = new Point ();
            n = new Normal ();
            dpdu = new Vector ();
            dpdv = new Vector ();
            dndu = new Normal ();
            dndv = new Normal ();
            dpdx = new Vector ();
            dpdy = new Vector ();
            u = v = 0.0;
            dudx = dudy = dvdx = dvdy = 0.0;
            Shape = null;
        }

        public DifferentialGeometry (Point p, Vector dpdu, Vector dpdv, Normal dndu, Normal dndv, double u, double v, IShape shape)
        {
            this.p = new Point (p);
            this.dpdu = new Vector (dpdu);
            this.dpdv = new Vector (dpdv);
            this.dndu = new Normal (dndu);
            this.dndv = new Normal (dndv);
            this.n = new Normal ((dpdu % dpdv).Normalized);
            this.u = u;
            this.v = v;
            dudx = dvdx = dudy = dvdy = 0.0;
            this.Shape = shape;
            
            if (shape != null && (shape.ReverseOrientation ^ shape.TransformSwapsHandedness))
                n *= -1.0;
        }

        public DifferentialGeometry (DifferentialGeometry dg)
        {
            this.p = new Point (dg.p);
            this.dpdu = new Vector (dg.dpdu);
            this.dpdv = new Vector (dg.dpdv);
            this.dndu = new Normal (dg.dndu);
            this.dndv = new Normal (dg.dndv);
            this.n = new Normal (dg.n);
            this.u = dg.u;
            this.v = dg.v;
            this.dudx = dg.dudx;
            this.dvdx = dg.dvdx;
            this.dudy = dg.dudy;
            this.dvdy = dg.dvdy;
            this.Shape = dg.Shape;
        }

        public void ComputeDifferentials (RayDifferential ray)
        {
            if (ray.HasDifferentials)
            {
                // Estimate screen space change in $\pt{}$ and $(u,v)$
                
                // Compute auxiliary intersection points with plane
                double d = -(n ^ new Vector (p.x, p.y, p.z));
                Vector rxv = new Vector (ray.RxOrigin.x, ray.RxOrigin.y, ray.RxOrigin.z);
                double tx = -((n ^ rxv) + d) / (n ^ ray.RxDirection);
                if (double.IsNaN (tx))
                {
                    dudx = dvdx = 0.0;
                    dudy = dvdy = 0.0;
                    dpdx = new Vector ();
                    dpdy = new Vector ();
                }
                Point px = ray.RxOrigin + tx * ray.RxDirection;
                Vector ryv = new Vector (ray.RyOrigin.x, ray.RyOrigin.y, ray.RyOrigin.z);
                double ty = -((n ^ ryv) + d) / (n ^ ray.RyDirection);
                if (double.IsNaN (ty))
                {
                    dudx = dvdx = 0.0;
                    dudy = dvdy = 0.0;
                    dpdx = new Vector ();
                    dpdy = new Vector ();
                }
                Point py = ray.RyOrigin + ty * ray.RyDirection;
                dpdx = px - p;
                dpdy = py - p;
                
                // Compute $(u,v)$ offsets at auxiliary points
                
                // Initialize _A_, _Bx_, and _By_ matrices for offset computation
                double[][] A = new double[2][];
                A[0] = new double[2];
                A[1] = new double[2];
                double[] Bx = new double[2], By = new double[2];
                int[] axes = new int[2];
                if (Math.Abs (n.x) > Math.Abs (n.y) && Math.Abs (n.x) > Math.Abs (n.z))
                {
                    axes[0] = 1;
                    axes[1] = 2;
                } else if (Math.Abs (n.y) > Math.Abs (n.z))
                {
                    axes[0] = 0;
                    axes[1] = 2;
                } else
                {
                    axes[0] = 0;
                    axes[1] = 1;
                }
                
                // Initialize matrices for chosen projection plane
                A[0][0] = dpdu[axes[0]];
                A[0][1] = dpdv[axes[0]];
                A[1][0] = dpdu[axes[1]];
                A[1][1] = dpdv[axes[1]];
                Bx[0] = px[axes[0]] - p[axes[0]];
                Bx[1] = px[axes[1]] - p[axes[1]];
                By[0] = py[axes[0]] - p[axes[0]];
                By[1] = py[axes[1]] - p[axes[1]];
                if (!Util.SolveLinearSystem2x2 (A, Bx[0], Bx[1], out dudx, out dvdx))
                {
                    dudx = 0.0;
                    dvdx = 0.0;
                }
                if (!Util.SolveLinearSystem2x2 (A, By[0], By[1], out dudy, out dvdy))
                {
                    dudy = 0.0;
                    dvdy = 0.0;
                }
            } else
            {
                dudx = dvdx = 0.0;
                dudy = dvdy = 0.0;
                dpdx = new Vector ();
                dpdy = new Vector ();
            }
        }
    }
}
