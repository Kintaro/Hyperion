
using System;
using System.Threading;

namespace Hyperion.Core
{
    /// <summary>
    ///
    /// </summary>
    public class ProgressReporter
    {
        private double _frequency;
        private double _count;
        private int _plussesPrinted;
        private int _totalPlusses;
        private int _left;
        private int _right;
        private Timer _timer;
        private Mutex MutexLock = new Mutex ();

        /// <summary>
        ///
        /// </summary>
        /// <param name="totalWork">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="title">
        /// A <see cref="System.String"/>
        /// </param>
        public ProgressReporter (int totalWork, string title) : this(totalWork, title, 50)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="totalWork">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="title">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="barLength">
        /// A <see cref="System.Int32"/>
        /// </param>
        public ProgressReporter (int totalWork, string title, int barLength)
        {
            _plussesPrinted = 0;
            _totalPlusses = barLength - title.Length;
            _frequency = (double)totalWork / (double)_totalPlusses;
            _count = _frequency;
            _timer = new Timer ();
            _timer.Start ();

            Console.Write ("  > " + title + ": [");
            _left = Console.CursorLeft;
            _right = _left + _totalPlusses;
            Console.CursorLeft = _right;
            Console.Write ("]");
            Console.CursorLeft = _left;
        }

        /// <summary>
        ///
        /// </summary>
        public void Update ()
        {
            Update (1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="num">
        /// A <see cref="System.Int32"/>
        /// </param>
        public void Update (int num)
        {
            lock (MutexLock)
            {
                _count -= num;

                Console.CursorLeft = _left;
                while (_count <= 0)
                {
                    _count += _frequency;
                    if (_plussesPrinted++ < _totalPlusses - 1)
                        Console.Write ("+");
                    else
                        Console.Write (">");
                    ++_left;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Done ()
        {
            lock (MutexLock)
            {
                Console.WriteLine ();
            }
        }

    }
}
