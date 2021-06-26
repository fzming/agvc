using System;
using System.Text;
using System.Threading;

namespace Utility.Helpers
{
    /// <summary>
    ///     按当前生产订单号生成器
    /// </summary>
    public static class DateTimeOrderNumberTool
    {
        private static long _np1, _np2, _np3 = 1; //临时计算用。

        private static readonly object
            locker = new(); //线程并行锁，以保证同一时间点只有一个用户能够操作流水号。如果分多个流水号段，放多个锁，并行压力可以更好的解决，大家自己想法子扩充吧

        /// <summary>
        ///     创建订单号码
        ///     编码规则：（16进制，从DateTime.MinValue起到此时的）
        ///     总天数 + 今天的总秒数 + 当前秒内产生的订单序号
        ///     ，其中今天的订单每秒清零。
        ///     该方法线程安全。
        /// </summary>
        public static string Create()
        {
            var now = DateTime.Now;
            var span = now - DateTime.MinValue;
            long tmpDays = span.Days;
            long seconds = span.Hours * 3600 + span.Seconds;
            var sb = new StringBuilder();
            Monitor.Enter(locker); //锁定资源
            if (tmpDays != _np1)
            {
                _np1 = tmpDays;
                _np2 = 0;
                _np3 = 1;
            }

            if (_np2 != seconds)
            {
                _np2 = seconds;
                _np3 = 1;
            }

            sb.Append(Convert.ToString(_np1, 16).PadLeft(5, '0') + Convert.ToString(_np2, 16).PadLeft(5, '0') +
                      Convert.ToString(_np3++, 16).PadLeft(6, '0'));
            Monitor.Exit(locker); //释放资源
            return sb.ToString().ToUpper();
        }

        /// <summary>
        ///     获取订单号表示的日期
        ///     即：反向获取订单号的日期
        /// </summary>
        public static DateTime ParseTime(string no)
        {
            if (!string.IsNullOrEmpty(no))
                return DateTime.MinValue.AddDays(Convert.ToInt64(no.Substring(0, 5), 16))
                    .AddSeconds(Convert.ToInt64("0x" + no.Substring(5, 5), 16));

            return DateTime.MinValue;
        }
    }
}