using System;
using System.Diagnostics;

namespace Utility.Helpers
{
    /// <summary>
    ///     StopwatchTimer
    /// </summary>
    public class StopwatchTimer : IDisposable
    {
        private readonly Stopwatch _codeStopwatch;
        private readonly string _codeUnderTime;
        private readonly Action<string> _logAction;

        public StopwatchTimer(string codeUnderTime) : this(codeUnderTime, Console.WriteLine)
        {
        }

        public StopwatchTimer(string codeUnderTime, Action<string> logAction)
        {
            _codeUnderTime = codeUnderTime;
            _logAction = logAction;
            _codeStopwatch = new Stopwatch();
            logAction($"[{DateTime.Now.ToShortTimeString()}] Starting {_codeUnderTime}");
            _codeStopwatch.Start();
        }


        public void Dispose()
        {
            _codeStopwatch.Stop();
            _logAction(
                $"[{DateTime.Now.ToShortTimeString()}] Finished {_codeUnderTime} in {_codeStopwatch.Elapsed.TotalMilliseconds}ms");
            _codeStopwatch.Reset();
        }
    }
}