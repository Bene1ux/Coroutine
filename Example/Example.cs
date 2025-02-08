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
        private static Stopwatch sw2;
        private static DateTime t=DateTime.Now;
        public static void Main()
        {
            //var seconds = CoroutineHandler.Start(Example.WaitSeconds(), "Awesome Waiting Coroutine");
            var seconds1 = CoroutineHandler.Start(Xdd1(), "asdf");
           /* CoroutineHandler.InvokeLater(new Wait(8000), () =>
            {
                Console.WriteLine("Raising test event");
                stop = true;
            });*/
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
            var sw= Stopwatch.StartNew();
            while (true)
            {
                var currTime = DateTime.Now;
                var delta= currTime - lastTime;
                //Console.WriteLine($"Delta: {delta.TotalMilliseconds}, elapsed {sw.ElapsedMilliseconds}");
                CoroutineHandler.Tick(delta);
                lastTime = currTime;
                Thread.Sleep(50);
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

                //Console.WriteLine($"Delay: {stopwatch.ElapsedMilliseconds} / {delayMs} ms");
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
            yield return new Wait(0);
            sw2 = Stopwatch.StartNew();
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"xx0 {stopwatch.ElapsedMilliseconds} / {(DateTime.Now-t).TotalMilliseconds}");
            Console.WriteLine($"xx1 {stopwatch.ElapsedMilliseconds} / {(DateTime.Now - t).TotalMilliseconds}");
            yield return Xdd2(stopwatch);
            Console.WriteLine($"xx4 {stopwatch.ElapsedMilliseconds} / {(DateTime.Now - t).TotalMilliseconds}");
        }

        private static IEnumerator Xdd2(Stopwatch stopwatch)
        {
            Console.WriteLine($"xx2 {stopwatch.ElapsedMilliseconds} / {(DateTime.Now - t).TotalMilliseconds}");
            yield return new Wait(20);
            Console.WriteLine($"xx3 {stopwatch.ElapsedMilliseconds} / {(DateTime.Now - t).TotalMilliseconds}");
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