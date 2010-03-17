
using System;
using System.Collections.Generic;
using System.Threading;

namespace Hyperion.Core.Parallel
{
    public static class ParallelUtility
    {
        public static Thread[] Threads;
        public static Queue<ITask> TaskQueue;
        public static Mutex TaskMutex;
        public static Mutex TaskQueueMutex;
        public static ConditionVariable TasksRunningCondition;
        public static int NumberOfUnfinishedTasks;

        public static int NumberOfSystemCores
        {
            get
            {
                return Environment.ProcessorCount;
            }
        }

        public static void EnqueueTasks (List<ITask> tasks)
        {
            if (Threads == null)
                TasksInit ();
            lock (TaskQueueMutex)
            {
                foreach (ITask task in tasks)
                    TaskQueue.Enqueue (task);
            }
            TasksRunningCondition.Lock ();
            NumberOfUnfinishedTasks += tasks.Count;
            TasksRunningCondition.Unlock ();
        }

        public static void WaitForAllTasks ()
        {
            TasksRunningCondition.Lock ();
            while (NumberOfUnfinishedTasks > 0)
                TasksRunningCondition.Wait ();
            TasksRunningCondition.Unlock ();
        }

        private static void TasksInit ()
        {
            Threads = new Thread[NumberOfSystemCores];
            for (int i = 0; i < NumberOfSystemCores; ++i)
                Threads[i] = new Thread (TaskEntry);
        }

        private static void TaskEntry ()
        {
            while (true)
            {
                ITask task = null;
                lock (TaskQueueMutex)
                {
                    if (TaskQueue.Count == 0)
                        break;
                    task = TaskQueue.Dequeue ();
                }

                task.Run ();
                TasksRunningCondition.Lock ();
                int unfinished = --NumberOfUnfinishedTasks;
                if (unfinished == 0)
                    TasksRunningCondition.Signal ();
                TasksRunningCondition.Unlock ();
            }
        }

        public static void AtomicAdd<T> (ref T destination, T source)
        {
            destination = source;
        }
    }
}