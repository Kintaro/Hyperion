
using System;

namespace Hyperion.Core
{
    public class Timer
    {
        private double time0, elapsed;
        private bool running;

        public Timer ()
        {
            time0 = elapsed = 0.0;
            running = false;
        }

        public void Start ()
        {
            running = true;
            time0 = GetTime ();
        }

        public void Stop ()
        {
            running = false;
            elapsed += GetTime () - time0;
        }

        public void Reset ()
        {
            running = false;
            elapsed = 0;
        }

        public double Time
        {
            get
            {
                if (running)
                {
                    Stop ();
                    Start ();
                }
                return elapsed;
            }
        }

        private double GetTime ()
        {
            return DateTime.Now.Second + DateTime.Now.Millisecond / 1000000.0;
        }
    }
}
