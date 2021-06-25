using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utility.Extensions
{
    /// <summary>
    /// 日期扩展
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class DateExtensions
    {
        #region UTC时间戳互转

        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="unixTimeStamp">Unix时间戳、UTC时间戳</param>
        /// <returns></returns>
        public static DateTime ToUnixDatetime(this string unixTimeStamp)
        {

            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1),TimeZoneInfo.Local);
            var ts = unixTimeStamp;
            var total = double.Parse(ts);
            if (ts.Length == 13) //13位时间戳转换成10位
            {
                total = total / 1000D;
            }
            return startTime.AddSeconds(total);

        }
        public static DateTime ToUnixDatetime(this long unixTimeStamp)
        {

            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1),TimeZoneInfo.Local);
            var ts = unixTimeStamp.ToString();
            var total = (double)unixTimeStamp;
            if (ts.Length == 13) //13位时间戳转换成10位
            {
                total = total / 1000D;
            }

            return startTime.AddSeconds(total);

        }
        /// <summary>
        /// JS时间戳转具体时间
        /// </summary>
        /// <param name="timestamp">JS时间戳</param>
        /// <returns></returns>
        public static DateTime ToDateTimeFromJsTimeStamp(this long timestamp)
        {
            var dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var lTime = long.Parse(timestamp + "0000");  //说明下，时间格式为13位后面补加4个"0"，如果时间格式为10位则后面补加7个"0"
            var toNow = new TimeSpan(lTime);
            var dtResult = dtStart.Add(toNow); //得到转换后的时间
            return dtResult;
        }
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式/UTC时间戳</returns>
        public static long ToUnixTimeStamp(this DateTime time)
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 获取JS时间戳
        /// </summary>
        /// <param name="time">[可选]转时间戳的日期，默认等于当前时间</param>
        /// <returns></returns>
        public static long ToJsTimestamp(this DateTime time)
        {
            var v = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var timestamp = (time.Ticks - v.Ticks) / 10000;//除10000调整为13位
            return timestamp;
        }
        #endregion

        /// <summary>
        /// 是否是正确的年月格式，如：201333 是错误的月份和日期
        /// </summary>
        /// <param name="ymt"></param>
        /// <returns></returns>
        public static bool IsValidDate(this string ymt)
        {
            if (ymt.Length != 6) return false;
            try
            {
                var date = new DateTime(
                    int.Parse("20" + ymt.Substring(0, 2)),
                    int.Parse(ymt.Substring(2, 2)),
                    int.Parse(ymt.Substring(4, 2)));

                return date.ToString("yyMMdd") == ymt;

            }
            catch (Exception)
            {
                return false;
            }
            
            
        }

        /// <summary>
        /// 一年有几周
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int MaxWeekOfYear(this int year)
        {
            if (year < 1 || year > 9999)
                throw new ArgumentException("illegal year", nameof(year));

            var maxDays = (DateTime.IsLeapYear(year) ? 366 : 365);

            var firstDayOfWeek = new DateTime(year, 1, 1).DayOfWeek;

            var beforeFirstSunday = (7 - Convert.ToInt32(firstDayOfWeek)) % 7;

            var remainDays = maxDays - beforeFirstSunday;

            var ret = (beforeFirstSunday % 7 == 0 ? 0 : 1);
            ret += (remainDays / 7);
            ret += (remainDays % 7 == 0 ? 0 : 1);

            return ret;
        }

        /// <summary>
        /// 获取指定日期所在年的第几周
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int WeekOfYear(this DateTime dt)
        {

            var gc = new GregorianCalendar();
            var weekOfYear = gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }
        /// <summary>
        /// 解析年份和周数
        /// </summary>
        /// <param name="strWeek"></param>
        /// <returns></returns>
        public static (int year, int week) ParseWeek(this string strWeek)
        {
            try
            {
                var reg = Regex.Match(strWeek,
                    @"(\d{4}).*?(\d{1,2})",
                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Multiline);
                var year = int.Parse(reg.Groups[1].Value);
                var week = int.Parse(reg.Groups[2].Value);

                return (year, week);

            }
            catch (ArgumentException)
            {
                // Syntax error in the regular expression
                throw new Exception("无法解析指定的周数时间");
            }
        }
        /// <summary>
        /// 周数转日期
        /// </summary>
        /// <param name="strWeek">2019-01</param>
        /// <returns></returns>
        public static DateTime[] WeekToDate(this string strWeek)
        {

            var tuple = strWeek.ParseWeek();
            var year = tuple.year;
            var week = tuple.week;

            //本年元旦是周几
            var dw = (int)DateTime.Parse(year + "-01-01").DayOfWeek;

            var monDays = (week - 1) * 7 - dw + 1;//所在周一距离元旦天数
            monDays = monDays < 0 ? 0 : monDays;
            var sunDays = (week) * 7 - dw;   //所在周日距离元旦天数

            var start = DateTime.Parse(year + "-01-01").AddDays(monDays);
            var end = DateTime.Parse(year + "-01-01").AddDays(sunDays);

            return new[] { start, end };
        }
        /// <summary>
        ///  自动分析年数和周数并格式化
        /// </summary>
        /// <param name="strWeek">包含年周的字符串，将自动分析年数和周数</param>
        /// <param name="format">默认格式：%Y-%U</param>
        /// <returns></returns>
        public static string FormatWeek(this string strWeek, string format = "%Y-%U")
        {
            //%Y-%U
            var tuple = strWeek.ParseWeek();
            var year = tuple.year;
            var week = tuple.week;

            return format.Replace("%Y", year.ToString()).Replace("%U", week.PadLeftZero());
        }
        /// <summary>
        /// 返回与当前时间对比相差的秒数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double Ttl(this DateTime time)
        {
            return DateTime.Now.Subtract(time).TotalSeconds;
        }
        /// <summary>
        /// 返回与当前时间对比相差的毫秒数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double TtlMs(this DateTime time)
        {
            return DateTime.Now.Subtract(time).TotalMilliseconds;
        }public static double TtlMs(this DateTime? time)
        {
            if (time == null) return 0;
            return DateTime.Now.Subtract(time.Value).TotalMilliseconds;
        }
        /// <summary>
        /// 返回与当前时间对比相差的天数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int TtlDays(this DateTime? time)
        {
            if (!time.HasValue)
            {
                return 0;
            }
            return (int)DateTime.Now.Subtract(time.Value).TotalDays;
        }
        /// <summary>
        /// 返回与当前时间对比相差的天数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int TtlDays(this DateTime time)
        {

            return (int)DateTime.Now.Subtract(time).TotalDays;
        }

        public static int TotalMonths(this DateTime start, DateTime end)
        {
            return (start.Year * 12 + start.Month) - (end.Year * 12 + end.Month);
        }

        /// <summary>
        /// 相差的月份
        /// </summary>
        /// <param name="d0">起始时间</param>
        /// <param name="d1">结束时间</param>
        /// <returns></returns>
        public static IEnumerable<DateTime> MonthsBetween(DateTime d0, DateTime d1)
        {
            return Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m));
        }

        /// <summary>
        /// 返回日期间隔年数
        /// </summary>
        /// <param name="lValue"></param>
        /// <param name="rValue"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static int TotalYears(DateTime lValue, DateTime rValue)
        {
            return lValue.Year - rValue.Year +
                   (lValue.Month > rValue.Month // Partial month, same year
                       ? 1
                       : ((lValue.Month == rValue.Month)
                          && (lValue.Day > rValue.Day)) // Partial month, same year and month
                           ? 1 : 0);
        }
        /// <summary>
        /// 取得某月的第一天
        /// </summary>
        /// <param name="datetime">要取得月份第一天的时间</param>
        /// <param name="edgeTime">使用边界时间</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(this DateTime datetime, bool edgeTime = false)
        {
            var t = datetime.AddDays(1 - datetime.Day);
            return edgeTime ? t.Start() : t;
        }

        /// <summary>
        /// 取得某月的最后一天
        /// </summary>
        /// <param name="datetime">要取得月份最后一天的时间</param>
        /// <param name="edgeTime">使用边界时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(this DateTime datetime, bool edgeTime = false)
        {
            var t = datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
            return edgeTime ? t.End() : t;
        }

        /// <summary>
        /// 取得上个月第一天
        /// </summary>
        /// <param name="datetime">要取得上个月第一天的当前时间</param>
        /// <returns></returns>
        public static DateTime FirstDayOfPreviousMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(-1);
        }

        /// <summary>
        /// 取得上个月的最后一天
        /// </summary>
        /// <param name="datetime">要取得上个月最后一天的当前时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfPreviousMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1);
        }
        /// <summary>
        /// 本周的第一天
        /// </summary>
        /// <param name="dateTime">要取的周起始天数的时间</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static DateTime FirstDayOfWeek(this DateTime dateTime)
        {

            return DateTime.Now.AddDays((-1) * Convert.ToInt32(dateTime.DayOfWeek) + 1);

        }
        /// <summary>
        /// 本周最后一天
        /// </summary>
        /// <param name="dateTime">要取的周结束天数的时间</param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime dateTime)
        {
            return DateTime.Now.AddDays(7 - Convert.ToInt32(dateTime.DayOfWeek));
        }

        public static DateTime ParseTime(this string dateString, string format = "yyyy-MM-dd hh:mm:ss")
        {
            return DateTime.ParseExact(dateString, format, CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// 获取某日的最早时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime Start(this DateTime dateTime)
        {
            return Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 00:00:00"));
        }
        /// <summary>
        /// 获取某日的最后时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime End(this DateTime dateTime)
        {
            return Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 23:59:59"));
        }
        /// <summary>
        /// 是否是无效时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsNotValid(this DateTime dateTime)
        {

            return dateTime <= Convert.ToDateTime("0001-1-1 8:00:00");
        }
        /// <summary>
        /// 是否已过期
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsExpires(this DateTime dateTime)
        {
            return dateTime < DateTime.Now;
        }
        public static bool IsNotValid(this DateTime? dateTime)
        {
            if (dateTime == null) return true;
            return dateTime <= Convert.ToDateTime("0001-1-1 8:00:00");
        }

        public static string ToShortDateString(this DateTime? time)
        {
            return time.HasValue ? time.Value.ToShortDateString() : string.Empty;
        }
        public static string ToDateString(this DateTime? time,string format)
        {
            return time.HasValue ? time.Value.ToString(format) : string.Empty;
        }
    }
}