
using System;
using System.Collections.Generic;

namespace Hyperion.Core.Parallel
{
    public static class ParallelUtility
    {
        public static int NumberOfSystemCores
        {
            get
            {
                return Environment.ProcessorCount;
            }
        }

        public static void EnqueueTasks (List<ITask> tasks)
        {
        }

        public static void WaitForAllTasks ()
        {
            
        }
    }
}
