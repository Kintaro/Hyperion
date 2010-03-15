
using System;

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
        }

        public DifferentialGeometry (Point p, Vector dpdu, Vector dpdv, Normal dndu, Normal dndv, double u, double v)
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
        }

        public void ComputeDifferentials (RayDifferential r)
        {

        }
    }
}
