using System;
using Hyperion.Core.Geometry;

namespace Hyperion
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Random random = new Random ();

            DateTime start = DateTime.Now;
            for (int i = 0; i < 50000; ++i)
            {
                Matrix a = new Matrix (random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (),
                random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble ());
                Matrix b = new Matrix (random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (),
                random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble (), random.NextDouble ());

                Matrix c = a * b;
                a = c;
                b = c;
            }
            DateTime end = DateTime.Now;

            System.Console.WriteLine ("Computation took {0}", end - start);
        }
    }
}
