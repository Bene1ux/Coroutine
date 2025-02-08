using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Coroutine
{
    /// <summary>
    /// A reference to a currently running coroutine.
    /// This is returned by <see cref="CoroutineHandler.Start(System.Collections.Generic.IEnumerator{Coroutine.Wait},string,int)"/>.
    /// </summary>
    public class ActiveCoroutine : IComparable<ActiveCoroutine>
    {
        private readonly IEnumerator enumerator;
        private readonly Stopwatch stopwatch;
        private Wait current;
        private readonly Stack<IEnumerator> coroutineStack = new Stack<IEnumerator>();

        internal Event Event => this.current.Event;
        internal bool IsWaitingForEvent => this.Event != null;

        /// <summary>
        /// This property stores whether or not this active coroutine is finished.
        /// A coroutine is finished if all of its waits have passed, or if it <see cref="WasCanceled"/>.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// This property stores whether or not this active coroutine was cancelled using <see cref="Cancel"/>.
        /// </summary>
        public bool WasCanceled { get; private set; }

        /// <summary>
        /// The total amount of time that <see cref="MoveNext"/> took.
        /// This is the amount of time that this active coroutine took for the entirety of its "steps", or yield statements.
        /// </summary>
        public TimeSpan TotalMoveNextTime { get; private set; }

        /// <summary>
        /// The total amount of times that <see cref="MoveNext"/> was invoked.
        /// This is the amount of "steps" in your coroutine, or the amount of yield statements.
        /// </summary>
        public int MoveNextCount { get; private set; }

        /// <summary>
        /// The amount of time that the last <see cref="MoveNext"/> took.
        /// This is the amount of time that this active coroutine took for the last "step", or yield statement.
        /// </summary>
        public TimeSpan LastMoveNextTime { get; private set; }

        /// <summary>
        /// An event that gets fired when this active coroutine finishes or gets cancelled.
        /// When this event is called, <see cref="IsFinished"/> is always true.
        /// </summary>
        public event FinishCallback OnFinished;

        /// <summary>
        /// The name of this coroutine.
        /// When not specified on startup of this coroutine, the name defaults to an empty string.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The priority of this coroutine. The higher the priority, the earlier it is advanced compared to other coroutines that advance around the same time.
        /// When not specified at startup of this coroutine, the priority defaults to 0.
        /// </summary>
        public readonly int Priority;

        internal ActiveCoroutine(IEnumerator enumerator, string name, int priority, Stopwatch stopwatch)
        {
            this.enumerator = enumerator;
            this.coroutineStack.Push(enumerator); // Add the initial coroutine
            this.Name = name;
            this.Priority = priority;
            this.stopwatch = stopwatch;
        }

        /// <summary>
        /// Cancels this coroutine, causing all subsequent <see cref="Wait"/>s and any code in between to be skipped.
        /// </summary>
        /// <returns>Whether the cancellation was successful, or this coroutine was already cancelled or finished</returns>
        public bool Cancel()
        {
            if (this.IsFinished || this.WasCanceled)
                return false;
            this.WasCanceled = true;
            this.IsFinished = true;
            this.OnFinished?.Invoke(this);
            return true;
        }

        internal bool Tick(double deltaMs)
        {
            while (!this.WasCanceled && this.current.Tick(deltaMs))
            {

                /*if (deltaMs > 40)
                {
                    Console.WriteLine($"Deltams: {deltaMs}");
                }*/
                if (!this.MoveNext())
                {
                    break;
                }

                if (!this.current.CanSpin())
                {
                    break;
                }

                this.current.SetSpinOnce();
                deltaMs = 0;
            }

            return this.IsFinished;
        }

        internal bool OnEvent(Event evt)
        {
            if (!this.WasCanceled && object.Equals(this.current.Event, evt))
                this.MoveNext();
            return this.IsFinished;
        }

        internal bool MoveNext()
        {
            this.stopwatch.Restart();
            while (coroutineStack.Count > 0)
            {
                var currentEnumerator = coroutineStack.Peek();
                var result = currentEnumerator.MoveNext();
                this.stopwatch.Stop();
                this.LastMoveNextTime = this.stopwatch.Elapsed;
                this.TotalMoveNextTime += this.stopwatch.Elapsed;
                this.MoveNextCount++;

                if (!result)
                {
                    coroutineStack.Pop(); // Nested coroutine finished
                    continue;
                }

                var currentYield = currentEnumerator.Current;

                // Handle nested coroutine
                if (currentYield is IEnumerator nestedCoroutine)
                {
                    coroutineStack.Push(nestedCoroutine);
                    continue;
                }

                // Handle Wait
                if (currentYield is Wait wait)
                {
                    this.current = wait;
                    return true;
                }

                // If the yield is neither, skip (or handle as needed)
            }

            this.IsFinished = true;
            this.OnFinished?.Invoke(this);
            return false;
        }

        /// <summary>
        /// A delegate method used by <see cref="ActiveCoroutine.OnFinished"/>.
        /// </summary>
        /// <param name="coroutine">The coroutine that finished</param>
        public delegate void FinishCallback(ActiveCoroutine coroutine);

        /// <inheritdoc />
        public int CompareTo(ActiveCoroutine other)
        {
            return other.Priority.CompareTo(this.Priority);
        }
    }
}