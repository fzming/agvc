using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Utility.Extensions
{
    /// <summary>
    ///     Enum扩展
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class EnumExtension
    {
        /// <summary>
        ///     包含是否包含Enum值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enums"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsValue<T>(this IEnumerable<T> enums, int value) where T : Enum
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            return enums.Any(p => Enum.IsDefined(typeof(T), p));
        }

        /// <summary>
        ///     将整形转换成Enum类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int value) where T : Enum
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            return (T) Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        ///     根据名称转换成Enum类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string name) where T : Enum
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            return (T) Enum.Parse(typeof(T), name, true);
        }

        public static Dictionary<string, int> ToDictionary(this Type t)
        {
            if (!t.IsEnum) throw new ArgumentException("T must be an enumerated type");

            return Enum.GetValues(t).Cast<Enum>().ToDictionary(item => item.ToString(), item => (int) (object) item);
        }

        public static Dictionary<string, int> ToDictionary<T>(this T t) where T : Enum
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToDictionary(fi => fi.Name,
                    fi => (int) fi.GetRawConstantValue());
        }


        public static IEnumerable<T> AsEnumerable<T>(this T t) where T : Enum
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> AsEnumerable<T>(this Type t)
        {
            if (!t.IsEnum) throw new ArgumentException("T must be an enumerated type");

            return Enum.GetValues(t).Cast<T>();
        }

        public static bool ContainsFlag(this Enum source, Enum flag)
        {
            var sourceValue = ToUInt64(source);
            var flagValue = ToUInt64(flag);

            return (sourceValue & flagValue) == flagValue;
        }

        public static bool ContainsAnyFlag(this Enum source, params Enum[] flags)
        {
            var sourceValue = ToUInt64(source);

            return flags.Select(ToUInt64).Any(flagValue => (sourceValue & flagValue) == flagValue);
        }

        // found in the Enum class as an internal method
        private static ulong ToUInt64(object value)
        {
            switch (Convert.GetTypeCode(value))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (ulong) Convert.ToInt64(value, CultureInfo.InvariantCulture);

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }

            throw new InvalidOperationException("Unknown enum type.");
        }
    }
}