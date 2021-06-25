using System;
using System.Linq;

namespace Utility.Extensions
{
    public static class ValueCastExtension
    {
        /// <summary>
        /// 遍历类型实现的所有接口，判断是否存在某个接口是泛型，且是参数中指定的原始泛型的实例
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="generic">泛型类型</param>
        /// <returns></returns>
        public static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
      
            return type.GetInterfaces().Any(x => generic == (x.IsGenericType ? x.GetGenericTypeDefinition() : x));
        }

        #region 数值转换
        /// <summary>
        /// 转换为长整型
        /// </summary>
        /// <param name="data">数据</param>
        public static long ToLong(this object data)
        {
            if (data == null)
                return 0;
            var success = long.TryParse(data.ToString(), out var result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt32(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换为可空长整型
        /// </summary>
        /// <param name="data">数据</param>
        public static long? ToLongOrNull(this object data)
        {
            if (data == null)
                return null;
            var isValid = long.TryParse(data.ToString(), out var result);
            if (isValid)
                return result;
            return null;
        }
        /// <summary>
        /// 转换为整型
        /// </summary>
        /// <param name="data">数据</param>
        public static int ToInt(this object data)
        {
            switch (data)
            {
                case null:
                    return 0;
                case Enum:
                    return Convert.ToInt32(data);
            }

            var success = int.TryParse(data.ToString(), out var result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt32(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换为可空整型
        /// </summary>
        /// <param name="data">数据</param>
        public static int? ToIntOrNull(this object data)
        {
            if (data == null)
                return null;
            var isValid = int.TryParse(data.ToString(), out var result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static double ToDouble(this object data)
        {
            if (data == null)
                return 0;
            return double.TryParse(data.ToString(), out var result) ? result : 0;
        }

        /// <summary>
        /// 转换为双精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static double ToDouble(this object data, int digits)
        {
            return Math.Round(ToDouble(data), digits);
        }

        /// <summary>
        /// 转换为可空双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static double? ToDoubleOrNull(this object data)
        {
            if (data == null)
                return null;
            var isValid = double.TryParse(data.ToString(), out var result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static decimal ToDecimal(this object data)
        {
            if (data == null)
                return 0;
            return decimal.TryParse(data.ToString(), out var result) ? result : 0;
        }

        /// <summary>
        /// 转换为高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static decimal ToDecimal(this object data, int digits)
        {
            return Math.Round(ToDecimal(data), digits);
        }

        /// <summary>
        /// 转换为可空高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static decimal? ToDecimalOrNull(this object data)
        {
            if (data == null)
                return null;
            var isValid = decimal.TryParse(data.ToString(), out var result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为可空高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static decimal? ToDecimalOrNull(this object data, int digits)
        {
            var result = ToDecimalOrNull(data);
            if (result == null)
                return null;
            return Math.Round(result.Value, digits);
        }

        #endregion

        #region 日期转换
        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="data">数据</param>
        public static DateTime ToDate(this object data)
        {
            if (data == null)
                return DateTime.MinValue;
            return DateTime.TryParse(data.ToString(), out var result) ? result : DateTime.MinValue;
        }

        /// <summary>
        /// 转换为可空日期
        /// </summary>
        /// <param name="data">数据</param>
        public static DateTime? ToDateOrNull(this object data)
        {
            if (data == null)
                return null;
            try
            {
                var isValid = DateTime.TryParse(data.ToString(), out var result);
                if (isValid)
                    return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
            return null;
        }

        #endregion

        #region 布尔转换
        /// <summary>
        /// 转换为布尔值
        /// </summary>
        /// <param name="data">数据</param>
        public static bool ToBool(this object data)
        {
            if (data == null)
                return false;
            var value = GetBool(data);
            if (value != null)
                return value.Value;
            return bool.TryParse(data.ToString(), out var result) && result;
        }

        /// <summary>
        /// 获取布尔值
        /// </summary>
        private static bool? GetBool(this object data)
        {
            switch (data.ToString().Trim().ToLower())
            {
                case "0":
                    return false;
                case "1":
                    return true;
                case "是":
                    return true;
                case "否":
                    return false;
                case "yes":
                    return true;
                case "no":
                    return false;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 转换为可空布尔值
        /// </summary>
        /// <param name="data">数据</param>
        public static bool? ToBoolOrNull(this object data)
        {
            if (data == null)
                return null;
            var value = GetBool(data);
            if (value != null)
                return value.Value;
            var isValid = bool.TryParse(data.ToString(), out var result);
            if (isValid)
                return result;
            return null;
        }

        #endregion

        #region 字符串转换
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="data">数据</param>
        public static string ToString(this object data)
        {
            return data == null ? string.Empty : data.ToString().Trim();
        }
        #endregion

        /// <summary>
        /// 安全返回值
        /// </summary>
        /// <param name="value">可空值</param>
        public static T SafeValue<T>(this T? value) where T : struct
        {
            return value ?? default;
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this Guid? value)
        {
            return value == null || IsEmpty(value.Value);
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this Guid value)
        {
            return value == Guid.Empty;
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this object value)
        {
            return value == null || string.IsNullOrEmpty(value.ToString());
        }

    }
}