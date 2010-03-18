using System;
using Hyperion.Core;
using Hyperion.Core.Parallel;

namespace Hyperion
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("---------------------------------------------------------------------");
            Console.WriteLine ("Hyperion version {0} of {1} at {2} [Detected {3} core(s)]", Api.Version, Api.VersionDate.Date.ToShortDateString (), Api.VersionDate.TimeOfDay, ParallelUtility.NumberOfSystemCores);
            Console.WriteLine ("---------------------------------------------------------------------");
            Console.WriteLine ("System:  {0}", System.Environment.MachineName);
            Console.WriteLine ("Version: {0}", System.Environment.OSVersion);
            Console.WriteLine ("Domain:  {0}", System.Environment.UserDomainName);
            Console.WriteLine (".NET:    {0}", System.Environment.Version);
            Console.WriteLine ("---------------------------------------------------------------------");
            Console.WriteLine ();
            Hyperion.Core.Parser.MrtParser parser = new Hyperion.Core.Parser.MrtParser ();
            parser.Parse ("bunny.hsf");
        }
    }
}
