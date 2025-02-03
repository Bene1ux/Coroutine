using System;
using System.Diagnostics;

namespace Coroutine
{
    /// <summary>
    /// Represents either an amount of time, or an <see cref="Coroutine.Event"/> that is being waited for by an <see cref="ActiveCoroutine"/>.
    /// </summary>
    public class Wait
    {

        internal readonly Event Event;
        private double seconds;
        private Func<bool> condition;
        private bool doShortSpins;
        private double maxSpinTime = 20f / 1000;

        /// <summary>
        /// Creates a new wait that waits for the given <see cref="Coroutine.Event"/>.
        /// </summary>
        /// <param name="evt">The event to wait for</param>
        public Wait(Event evt)
        {
            this.Event = evt;
            this.seconds = 0;
        }

        /// <summary>
        /// Creates a new wait that waits for the given amount of seconds.
        /// </summary>
        /// <param name="seconds">The amount of seconds to wait for</param>
        public Wait(double seconds, bool doSpins = false)
        {
            this.seconds = seconds;
            this.Event = null;
            this.doShortSpins = doSpins;
        }

        public Wait(Func<bool> condition, double seconds = 5)
        {
            this.condition = condition;
            this.seconds = seconds;
            //this.doShortSpins = doSpins;
        }

        /// <summary>
        /// Creates a new wait that waits for the given <see cref="TimeSpan"/>.
        /// Note that the exact value may be slightly different, since waits operate in <see cref="TimeSpan.TotalSeconds"/> rather than ticks.
        /// </summary>
        /// <param name="time">The time span to wait for</param>
        public Wait(TimeSpan time) : this(time.TotalSeconds)
        {
        }

        private void Spin(double seconds)
        {
            var delayMs = seconds * 1000;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < delayMs)
            {

            }
        }

        internal bool Tick(double deltaSeconds)
        {
            seconds -= deltaSeconds;
            if (condition == null)
            {
                if (doShortSpins && seconds <= maxSpinTime)
                {
                    Spin(seconds);
                    seconds = 0;
                }
                return seconds <= 0;
            }

            if (condition())
            {

                return true;
            }

          /*  if (doShortSpins && seconds <= maxSpinTime)
            {
                Spin(seconds);
            }

            if (condition())
            {

                return true;
            }*/

            return this.seconds <= 0;
        }

    }
}