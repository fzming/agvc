using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Utility.Helpers
{
    /// <summary>
    ///     执行代码规范
    /// </summary>
    public interface IAction
    {
        void Action();
    }

    /// <summary>
    ///     老赵的性能测试工具
    /// </summary>
    public static class CodeTimer
    {
        public delegate void ActionDelegate();

        static CodeTimer()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime,
            out long lpKernelTime, out long lpUserTime);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        private static long GetCurrentThreadTimes()
        {
            GetThreadTimes(GetCurrentThread(), out _, out _, out var kernelTime, out var userTimer);
            return kernelTime + userTimer;
        }

        public static void Time(string name, int iteration, ActionDelegate action)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (action == null) return;

            //1. Print name
            var currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // 2. Record the latest GC counts
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.Collect(GC.MaxGeneration);
            var gcCounts = new int[GC.MaxGeneration + 1];
            for (var i = 0; i <= GC.MaxGeneration; i++) gcCounts[i] = GC.CollectionCount(i);

            // 3. Run action
            var watch = new Stopwatch();
            watch.Start();
            var ticksFst = GetCurrentThreadTimes(); //100 nanosecond one tick
            for (var i = 0; i < iteration; i++) action();
            var ticks = GetCurrentThreadTimes() - ticksFst;
            watch.Stop();

            // 4. Print CPU
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tTime Elapsed:\t\t" +
                              watch.ElapsedMilliseconds.ToString("N0") + "ms");
            Console.WriteLine("\tTime Elapsed (one time):" +
                              (watch.ElapsedMilliseconds / iteration).ToString("N0") + "ms");
            Console.WriteLine("\tCPU time:\t\t" + (ticks * 100).ToString("N0")
                                                + "ns");
            Console.WriteLine("\tCPU time (one time):\t" + (ticks * 100 /
                                                            iteration).ToString("N0") + "ns");

            // 5. Print GC
            for (var i = 0; i <= GC.MaxGeneration; i++)
            {
                var count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t\t" + count);
            }

            Console.WriteLine();
        }


        public static void Time(string name, int iteration, IAction action)
        {
            if (string.IsNullOrEmpty(name)) return;

            if (action == null) return;

            //1. Print name
            var currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // 2. Record the latest GC counts
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.Collect(GC.MaxGeneration);
            var gcCounts = new int[GC.MaxGeneration + 1];
            for (var i = 0; i <= GC.MaxGeneration; i++) gcCounts[i] = GC.CollectionCount(i);

            // 3. Run action
            var watch = new Stopwatch();
            watch.Start();
            var ticksFst = GetCurrentThreadTimes(); //100 nanosecond one tick
            for (var i = 0; i < iteration; i++) action.Action();
            var ticks = GetCurrentThreadTimes() - ticksFst;
            watch.Stop();

            // 4. Print CPU
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tTime Elapsed:\t\t" +
                              watch.ElapsedMilliseconds.ToString("N0") + "ms");
            Console.WriteLine("\tTime Elapsed (one time):" +
                              (watch.ElapsedMilliseconds / iteration).ToString("N0") + "ms");
            Console.WriteLine("\tCPU time:\t\t" + (ticks * 100).ToString("N0")
                                                + "ns");
            Console.WriteLine("\tCPU time (one time):\t" + (ticks * 100 /
                                                            iteration).ToString("N0") + "ns");

            // 5. Print GC
            for (var i = 0; i <= GC.MaxGeneration; i++)
            {
                var count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t\t" + count);
            }

            Console.WriteLine();
        }
    }
}