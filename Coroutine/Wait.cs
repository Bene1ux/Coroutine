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
        private double initialWaitMs;
        private Func<bool> condition;
        private bool doShortSpins;
        private bool spinOnce;
        private double maxSpinTime = 40;

        /// <summary>
        /// Creates a new wait that waits for the given <see cref="Coroutine.Event"/>.
        /// </summary>
        /// <param name="evt">The event to wait for</param>
        public Wait(Event evt)
        {
            this.Event = evt;
            this.waitMs = 0;
        }

        public bool CanSpin()
        {
            return false;
           // return doShortSpins&&initialWaitMs <= maxSpinTime;
        }

        public void SetSpinOnce()
        {
            spinOnce = true;
        }

        /// <summary>
        /// Creates a new wait that waits for the given amount of milliseconds.
        /// </summary>
        /// <param name="waitMs">The amount of milliseconds to wait for</param>
        public Wait(double waitMs, bool doSpins = true, int maxSpinTimeMs=40)
        {
            this.waitMs = waitMs;
            this.Event = null;
            this.initialWaitMs = waitMs;
            this.doShortSpins = doSpins;
            this.maxSpinTime = maxSpinTimeMs;
        }

        public Wait(Func<bool> condition, double maxWaitMs = 5000)
        {
            this.condition = condition;
            this.waitMs = maxWaitMs;
            this.initialWaitMs = waitMs;
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
                if (!spinOnce)
                {
                    return waitMs <= 0;
                }

                Spin(waitMs);
                spinOnce = false;
                waitMs = 0;

                return true;
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