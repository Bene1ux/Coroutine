using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Coroutine;

namespace Example
{
    internal static class Example
    {

        private static readonly Event TestEvent = new Event();
        private static bool stop = false;
        public static void Main()
        {
            //var seconds = CoroutineHandler.Start(Example.WaitSeconds(), "Awesome Waiting Coroutine");
            var seconds1 = CoroutineHandler.Start(Xdd1(), "asdf");
            CoroutineHandler.InvokeLater(new Wait(8000), () =>
            {
                Console.WriteLine("Raising test event");
                stop = true;
            });
            /* CoroutineHandler.Start(Example.PrintEvery10Seconds(seconds));
           
            CoroutineHandler.Start(Example.EmptyCoroutine());

            CoroutineHandler.InvokeLater(new Wait(5), () => {
                Console.WriteLine("Raising test event");
                CoroutineHandler.RaiseEvent(Example.TestEvent);
            });
            CoroutineHandler.InvokeLater(new Wait(Example.TestEvent), () => Console.WriteLine("Example event received"));

            CoroutineHandler.InvokeLater(new Wait(Example.TestEvent), () => Console.WriteLine("I am invoked after 'Example event received'"), priority: -5);
            CoroutineHandler.InvokeLater(new Wait(Example.TestEvent), () => Console.WriteLine("I am invoked before 'Example event received'"), priority: 2);
*/
            var lastTime = DateTime.Now;
            while (true)
            {
                var currTime = DateTime.Now;
                CoroutineHandler.Tick(currTime - lastTime);
                lastTime = currTime;
                Thread.Sleep(1);
            }
        }

        private static IEnumerator SleepTest()
        {
            var random = new Random();

            while (true)
            {
                yield return new Wait(1000);
                int delayMs = random.Next(1, 31);

                Stopwatch stopwatch = Stopwatch.StartNew();
                yield return new Wait(delayMs);

                stopwatch.Stop();

                Console.WriteLine($"Delay: {stopwatch.ElapsedMilliseconds} / {delayMs} ms");
            }

        }

        private static IEnumerator SleepTest2()
        {
            var random = new Random();

            while (true)
            {
                yield return new Wait(1000);
                int delayMs = random.Next(1, 31);

                Stopwatch stopwatch = Stopwatch.StartNew();
                while (stopwatch.ElapsedMilliseconds < delayMs)
                {

                }


                stopwatch.Stop();

                Console.WriteLine($"Delay: {stopwatch.ElapsedMilliseconds} / {delayMs} ms");
            }

        }

        private static IEnumerator Xdd0()
        {

            Console.WriteLine($"xx1");
            yield return new Wait(() => { return stop; });
            Console.WriteLine($"xx2 {stop}");
        }


        private static IEnumerator Xdd1()
        {
            Console.WriteLine($"xx1");
            yield return Xdd2();
            Console.WriteLine($"xx4");
        }

        private static IEnumerator Xdd2()
        {
            Console.WriteLine($"xx2");
            yield return new Wait(5000);
            Console.WriteLine($"xx3");
        }

        private static IEnumerator<Wait> WaitSeconds()
        {
            Console.WriteLine("First thing " + DateTime.Now);
            yield return new Wait(1000);
            Console.WriteLine("After 1 second " + DateTime.Now);
            yield return new Wait(9000);
            Console.WriteLine("After 10 seconds " + DateTime.Now);
            CoroutineHandler.Start(Example.NestedCoroutine());
            yield return new Wait(5000);
            Console.WriteLine("After 5 more seconds " + DateTime.Now);
            yield return new Wait(10000);
            Console.WriteLine("After 10 more seconds " + DateTime.Now);

            yield return new Wait(20000);
            Console.WriteLine("First coroutine done");
        }

        private static IEnumerator<Wait> PrintEvery10Seconds(ActiveCoroutine first)
        {
            while (true)
            {
                yield return new Wait(10000);
                Console.WriteLine("The time is " + DateTime.Now);
                if (first.IsFinished)
                {
                    Console.WriteLine("By the way, the first coroutine has finished!");
                    Console.WriteLine($"{first.Name} data: {first.MoveNextCount} moves, " +
                                      $"{first.TotalMoveNextTime.TotalMilliseconds} total time, " +
                                      $"{first.LastMoveNextTime.TotalMilliseconds} last time");
                    Environment.Exit(0);
                }
            }
        }

        private static IEnumerator<Wait> EmptyCoroutine()
        {
            yield break;
        }

        private static IEnumerable<Wait> NestedCoroutine()
        {
            Console.WriteLine("I'm a coroutine that was started from another coroutine!");
            yield return new Wait(5000);
            Console.WriteLine("It's been 5 seconds since a nested coroutine was started, yay!");
        }

    }
}