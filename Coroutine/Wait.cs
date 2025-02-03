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
        private double waitMs;
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
            this.waitMs = 0;
        }

        /// <summary>
        /// Creates a new wait that waits for the given amount of milliseconds.
        /// </summary>
        /// <param name="waitMs">The amount of milliseconds to wait for</param>
        public Wait(double waitMs, bool doSpins = false)
        {
            this.waitMs = waitMs;
            this.Event = null;
            this.doShortSpins = doSpins;
        }

        public Wait(Func<bool> condition, double maxWaitMs = 5000)
        {
            this.condition = condition;
            this.waitMs = maxWaitMs;
            //this.doShortSpins = doSpins;
        }

        /// <summary>
        /// Creates a new wait that waits for the given <see cref="TimeSpan"/>.
        /// Note that the exact value may be slightly different, since waits operate in <see cref="TimeSpan.TotalSeconds"/> rather than ticks.
        /// </summary>
        /// <param name="time">The time span to wait for</param>
        public Wait(TimeSpan time) : this(time.TotalMilliseconds)
        {
        }

        private void Spin(double ms)
        {
            var delayMs = ms;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < delayMs)
            {

            }
        }

        internal bool Tick(double deltaMs)
        {
            waitMs -= deltaMs;
            if (condition == null)
            {
                if (doShortSpins && waitMs <= maxSpinTime)
                {
                    Spin(waitMs);
                    waitMs = 0;
                }
                return waitMs <= 0;
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

            return this.waitMs <= 0;
        }

    }
}