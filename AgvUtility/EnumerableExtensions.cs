using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Utility
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class EnumerableExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum t) where
            TAttribute : Attribute
        {

            var type = t.GetType();
            var memInfo = type.GetMember(t.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(TAttribute), false);
                if (attrs.Length > 0)
                    return (TAttribute)attrs[0];
            }
            return default;
        }
        /// <summary>
        /// 使用hashtable形式快速去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// 使用GROUP形式快速去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctByGroup<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source
                .GroupBy(keySelector)
                .Select(g => g.First());
        }

        public static int FindIndexBy<TSource>(this List<TSource> source, Predicate<TSource> match,bool firstIndex)
        {
            return firstIndex ? source.FindIndex(match) : source.FindLastIndex(match);
        }
        public static string JoinToString<T>(this IEnumerable<T> ns, string separator = ",")
        {
            if (!ns.AnyNullable())
            {
                return string.Empty;
            }
            return string.Join(separator, ns);
        }

        public static string JoinToString<T>(this IEnumerable<T> ns, char separator = ',')
        {
            if (!ns.AnyNullable())
            {
                return string.Empty;
            }
            return string.Join(separator.ToString(), ns);
        }
        /// <summary>
        /// 随机抽取指定数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ns"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> ns, int count = 0)
        {
            var rs = ns.OrderBy(_ => Guid.NewGuid());
            return count > 0 ? rs.Take(count) : rs;
        }
        /// <summary>
        /// 随机抽取一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ns"></param>
        /// <returns></returns>
        public static T TakeOne<T>(this IEnumerable<T> ns)
        {
            return ns.TakeRandom(1).FirstOrDefault();
        }
        /// <summary>
        /// 可为NUll Any
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool AnyNullable<T>(this IEnumerable<T> source)
        {
            try
            {
                return source != null && source.Any();
            }
            catch
            {
                return false;
            }
            
        }
        public static bool IsIEnumerableOfT(this Type type)
        {
            return type.GetInterfaces()
                .Any(ti => ti.IsGenericType
                           && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }
        public static bool AnyNullable<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            try
            {
                if (source == null) return false;
                return predicate == null ? source.Any() : source.Any(predicate);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 防止Null ToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToListEx<T>(this IEnumerable<T> source)
        {
            return source == null ? Enumerable.Empty<T>().ToList() : source.ToList();
        }
        /// <summary>
        /// 可为NULL 的ANY ，并允许过滤NullOrEmpty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="filterNullOrEmpty"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool AnyNullable<T>(this IEnumerable<T> source, bool filterNullOrEmpty, Func<T, bool> predicate)
        {
            if (source == null)
            {
                return false;
            }
            var enumerable = source.ToList();
            var any = enumerable.AnyNullable(predicate);
            if (!filterNullOrEmpty)
            {
                return any;
            }

            return any && enumerable.Where(p => p != null).AnyNullable(predicate);
        }
        public static Task ForEachAsync<T>(this IEnumerable<T> sequence, Func<T, Task> selector)
        {
            return Task.WhenAll(sequence.Select(selector));
        }
        public static Task<TDis[]> ForEachAsync<T, TDis>(this IEnumerable<T> sequence, Func<T, Task<TDis>> selector)
        {
            return Task.WhenAll(sequence.Select(selector));
        }
        /// <summary>
        /// 分页异步执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="pageSize"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task ForEachPageAsync<T>(this IEnumerable<T> sequence, int pageSize, Func<T, Task> selector)
        {
            return sequence.CreatePageTaskAsync(pageSize, (pageIndex, pageDatas) => ForEachAsync(pageDatas, selector));

        }

        public static async Task CreatePageTaskAsync<T>(this IEnumerable<T> sequence, int pageSize, Func<int, IEnumerable<T>, Task> pageCallTask)
        {
            var enumerable = sequence.ToList();
            var total = enumerable.Count;
            var pageIndex = 1;

            if (pageSize > 0 && total > 0)
            {

                //计算总页面数
                var pageCount = (total + pageSize - 1) / pageSize;

                while (pageIndex <= pageCount)
                {

                    var pageDatas = enumerable.Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize);
#if DEBUG
                    Trace.WriteLine($"PageTask(({typeof(T).Name} Total:{total})):{pageIndex}/{pageCount}  PageSize:{pageDatas.Count()}");
#endif
                    await pageCallTask?.Invoke(pageIndex, pageDatas);
                    pageIndex++;
                }
            }
            else
            {
                await pageCallTask?.Invoke(pageIndex, enumerable);
            }
        }

        public static Task ForEachPageAsync<T>(this IEnumerable<T> sequence, int pageSize, Func<int, IEnumerable<T>, Task> pageTask)
        {
            return sequence.CreatePageTaskAsync(pageSize, pageTask);
        }

        /// <summary>
        /// 分页异步执行并返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDist"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="pageSize"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TDist>> ForEachPageAsync<T, TDist>(this IEnumerable<T> sequence,
            int pageSize,
            Func<T, Task<TDist>> selector)
        {
            var dtos = new List<TDist>();
            await sequence.CreatePageTaskAsync(pageSize, async (index, pageDatas) =>
            {
                var datas = await ForEachAsync(pageDatas, selector);
                dtos.AddRange(datas);
            });
            return dtos;
        }

        public static bool TryUpdateEx<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dict,
            TKey key,
            Func<TValue, TValue> updateFactory)
        {
            while (dict.TryGetValue(key, out var curValue))
            {
                if (dict.TryUpdate(key, updateFactory(curValue), curValue))
                    return true;
                // if we're looping either the key was removed by another thread,
                // or another thread changed the value, so we start again.
            }
            return false;
        }

        public static object TryGetValue<T>(this IList<T> list, int i)
        {
            if (i < 1 || i > list.Count - 1) return default(T);
            return list[i];
        }
    }
}