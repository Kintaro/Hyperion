
using System;
using System.Collections.Generic;
using System.Threading;

namespace Hyperion.Core.Parallel
{
    public static class ParallelUtility
    {
        public static Thread[] Threads;
        public static Queue<ITask> TaskQueue = new Queue<ITask> ();
        public static Mutex TaskMutex = new Mutex ();
        public static Mutex TaskQueueMutex = new Mutex ();
        public static ConditionVariable TasksRunningCondition = new ConditionVariable ();
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
            lock (TaskQueueMutex)
            {
                if (Threads == null)
                    TasksInit ();

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
            {
                (Threads[i] = new Thread (TaskEntry)).Start ();
            }
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