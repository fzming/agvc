using System;
using System.Timers;

namespace Utility.Helpers
{
    public static class AgvcTimer
    {
        /// <summary>
        ///     在指定时间过后执行指定的表达式
        /// </summary>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param>
        /// <param name="action">要执行的表达式</param>
        public static void SetTimeout(double interval, Action action)
        {
            var timer = new Timer(interval);
            timer.Elapsed += delegate
            {
                timer.Enabled = false;
                action();
            };
            timer.Enabled = true;
        }

        /// <summary>
        ///     在指定时间周期重复执行指定的表达式
        /// </summary>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param>
        /// <param name="action">要执行的表达式</param>
        public static void SetInterval(double interval, Action<ElapsedEventArgs> action)
        {
            var timer = new Timer(interval);
            timer.Elapsed += delegate(object sender, ElapsedEventArgs e) { action(e); };
            timer.Enabled = true;
        }
    }
}