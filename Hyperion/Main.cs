using System;
using Hyperion.Core;
using Hyperion.Core.Parallel;

namespace Hyperion
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hyperion version {0} of {1} at {2} [Detected {3} core(s)]", Api.Version, Api.VersionDate.Date, Api.VersionDate.TimeOfDay, ParallelUtility.NumberOfSystemCores);
            Hyperion.Core.Parser.MrtParser parser = new Hyperion.Core.Parser.MrtParser ();
            parser.Parse (args[0]);
        }
    }
}
