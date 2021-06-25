using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Utility.Extensions
{
    public static class ValueTypeExtensions
    {

        /// <summary>
        /// 编码为16位或32位MD5
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToMd5(this string s, Md5CryptLength length = Md5CryptLength.SHORT)
        {
            var md5 = new MD5CryptoServiceProvider();
            var b = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            var strB = new StringBuilder();
            foreach (var t in b)
                strB.Append(t.ToString("x").PadLeft(2, '0'));

            return length == Md5CryptLength.SHORT ? strB.ToString(8, 16) : strB.ToString();
        }
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }
        public static bool IsNotNullOrEmpty(this string s)
        {
            return !s.IsNullOrEmpty();
        }

        /// <summary>
        /// 不足10补0 转字符串
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string PadLeftZero(this int n)
        {
            return $"{n:d2}";
        }
        public static string ToStringEx(this object s, string defaultValue = "")
        {
            return s == null ? defaultValue : s.ToString();
        }
        /// <summary>
        ///  将毫秒数转换为 时分秒格式
        /// </summary>
        /// <param name="ms">毫秒数</param>
        /// <returns></returns>
        public static string ToTimeString(this long ms)
        {
            var t = TimeSpan.FromMilliseconds(ms);
            return t.ToString(@"hh\:mm\:ss\:fff");
        }
        /// <summary>
        /// 去除制表符等非字符串数据
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Strip(this string s)
        {
            return Regex.Replace(s, @"[\t\s\r\n\0\b]", "");
        }
        public static string CharAt(this string s, int index)
        {
            if ((index >= s.Length) || (index < 0))
                return "";
            return s.Substring(index, 1);
        }
        public static string UrlEncode(this string s)
        {
            return Uri.EscapeDataString(s);
            //   return HttpUtility.UrlEncode(s, Encoding.UTF8)?.Replace("+", "%20"); //解决空格变成加号的BUG
        }
        public static string UrlDecode(this string source, Encoding e = null)
        {
            return HttpUtility.UrlDecode(source, e ?? Encoding.UTF8);
        }
        /// <summary>
        /// 从字符串左边开始截取指定长度的字符串,源字符串长度不足时,则返回源字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string Left(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return str.Length <= length ? str : str.Substring(0, length);
        }

        /// <summary>
        /// 从字符串右边开始截取指定长度的字符串,源字符串长度不足时,则返回源字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string Right(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return str.Length <= length ? str : str.Substring(str.Length - length);
        }
        /// <summary>
        /// 处于某个区间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T s, T min, T max) where T : IComparable
        {
            return s.CompareTo(min) >= 0 && s.CompareTo(max) <= 0;
        }
    }
}