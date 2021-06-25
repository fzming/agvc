using System.Collections.Generic;
using System.Linq;

namespace CoreData.Core
{
    public static class MongoEntityExtension
    {
        /// <summary>
        /// MongoEntity实体集合克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="deep">是否进行深度克隆</param>
        /// <returns></returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> source, bool deep = false) where T : MongoEntity, new()
        {
            return deep ? source.Select(p => p.Clone(true)) : source.AutoMapIgnoreId<T, string>();
        }

        /// <summary>
        /// MongoEntity单实体克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="deep">是否进行深度克隆</param>
        /// <returns></returns>
        public static T Clone<T>(this T source, bool deep = false) where T : MongoEntity, new()
        {
            return deep ? source.DeepClone() : source.AutoMapIgnoreId<T, string>();
        }

    }
}