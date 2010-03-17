
using System;
using System.Threading;

namespace Hyperion.Core
{
    public class ConditionVariable
    {
        private Mutex Mutex = new Mutex ();
        private ThreadState Condition;

        public ConditionVariable ()
        {
        }

        public void Lock ()
        {
            lock (Mutex);
        }

        public void Unlock ()
        {

        }

        public void Wait ()
        {
            
        }

        public void Signal ()
        {
        }
    }
}
