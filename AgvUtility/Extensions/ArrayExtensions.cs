using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Utility.Extensions
{
    /// <summary>
    ///     数组对象扩展
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class ArrayExtensions
    {
        /// <summary>
        ///     从数组中安全读取指定位置元素（防溢出错误）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public static T GetValueEx<T>(this T[] arr, int index)
        {
            if (arr == null) return default;
            if (arr.Length == 0 || index > arr.Length - 1 || index < 0) return default;
            var type = typeof(T);
            var v = arr[index];

            if (type != typeof(string) && !type.IsValueType && (type.IsInterface || type.IsClass)) return v;
            try
            {
                return (T) TypeDescriptor.GetConverter(type).ConvertFrom(v);
            }
            catch (FormatException exception)
            {
                throw new FormatException($"值：{v}尝试转换成：{type.FullName}时错误，格式不正确\n{exception.Message}");
            }
        }


        /// <summary>
        ///     Array添加
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="array">Array</param>
        /// <param name="item">需要添加项</param>
        /// <returns>返回新的Array</returns>
        public static T[] Add<T>(this T[] array, T item)
        {
            var count = array.Length;
            Array.Resize(ref array, count + 1);
            array[count] = item;
            return array;
        }

        /// <summary>
        ///     从Array中移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, T item)
        {
            return array.Where(p => p.Equals(item) == false).ToArray();
        }

        /// <summary>
        ///     从Array中移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, Func<T, bool> predicate)
        {
            return array.Where(predicate).ToArray();
        }

        /// <summary>
        ///     Array添加块数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="sourceArray">Array</param>
        /// <param name="addArray">Array</param>
        /// <returns>返回新的Array</returns>
        public static T[] AddRange<T>(this T[] sourceArray, T[] addArray)
        {
            var count = sourceArray.Length;
            var addCount = addArray.Length;
            Array.Resize(ref sourceArray, count + addCount);

            addArray.CopyTo(sourceArray, count);
            return sourceArray;
        }

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        public static bool IsNullOrEmpty(this ArrayList list)
        {
            return list == null || list.Count == 0;
        }


        /// <summary>
        ///     Array 移除重复项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] RemoveDuplicates<T>(this T[] array)
        {
            var al = new ArrayList();
            foreach (var t in array)
                if (!al.Contains(t))
                    al.Add(t);
            return (T[]) al.ToArray(typeof(T));
        }

        /// <summary>
        ///     Array 数组分割
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] Slice<T>(this T[] array, int start, int end)
        {
            if (start >= array.Length)
            {
                start = 0;
                end = 0;
            }

            if (end < 0) end = array.Length - start - end;
            if (end <= start) end = start;
            if (end >= array.Length) end = array.Length - 1;
            var len = end - start + 1;
            var res = new T[len];
            for (var i = 0; i < len; i++) res[i] = array[i + start];
            return res;
        }
    }
}