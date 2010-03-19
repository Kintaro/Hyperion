
using System;
using System.Collections.Generic;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Interfaces
{
    public class ShapeSet
    {
        private double SumArea;
        private List<IShape> Shapes;
        private List<double> Areas;
        private Distribution1D AreaDistribution;

        public ShapeSet (IShape shape)
        {
            Areas = new List<double> ();
            Shapes = new List<IShape> ();
            List<IShape> todo = new List<IShape> ();
            todo.Add (shape);
            while (todo.Count > 0)
            {
                IShape sh = todo[todo.Count - 1];
                todo.RemoveAt (todo.Count - 1);
                if (sh.CanIntersect)
                    Shapes.Add (sh);
                else
                    sh.Refine (ref todo);
            }
            
            SumArea = 0.0;
            foreach (IShape s in Shapes)
            {
                double area = s.Area;
                Areas.Add (area);
                SumArea += area;
            }
            
            AreaDistribution = new Distribution1D (Areas, Areas.Count);
        }

        public Point Sample (Point p, LightSample lightSample, ref Normal Ns)
        {
            double temp = 0.0;
            int sn = AreaDistribution.SampleDiscrete (lightSample.uComponent, ref temp);
            Point pt = Shapes[sn].Sample (p, lightSample.uPos[0], lightSample.uPos[1], ref Ns);
            // Find closest intersection of ray with shapes in _ShapeSet_
            Ray r = new Ray (p, pt - p, 0.001, double.PositiveInfinity);
            double rayEps = 0.0, thit = 1.0;
            bool anyHit = false;
            DifferentialGeometry dg = new DifferentialGeometry ();
            foreach (IShape shape in Shapes)
                anyHit |= shape.Intersect (r, ref thit, ref rayEps, ref dg);
            if (anyHit)
                Ns = new Normal (dg.n);
            return r.Apply (thit);
        }

        public Point Sample (LightSample lightSample, ref Normal Ns)
        {
            double temp = 0.0;
            int sn = AreaDistribution.SampleDiscrete (lightSample.uComponent, ref temp);
            return Shapes[sn].Sample (lightSample.uPos[0], lightSample.uPos[1], ref Ns);
        }

        public double Pdf (Point p, Vector wi)
        {
            double pdf = 0.0;
            for (int i = 0; i < Shapes.Count; ++i)
                pdf += Areas[i] * Shapes[i].Pdf (p, wi);
            return pdf / SumArea;
        }

        public double Pdf (Point p)
        {
            double pdf = 0.0;
            for (int i = 0; i < Shapes.Count; ++i)
                pdf += Areas[i] * Shapes[i].Pdf (p);
            return pdf / SumArea;
        }

        public double Area {
            get { return SumArea; }
        }
    }
}
