using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace CoreRepository.Core
{
    public static class ObjectIdExtension
    {
        /// <summary>
        /// 判断字符串是否是BSON ObjectId
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsObjectId(this string input)
        {
            return !string.IsNullOrEmpty(input) && ObjectId.TryParse(input, out _);

        }
        public static bool IsObjectIdArray(this string[] inputs)
        {
            return inputs.All(p => p.IsObjectId());

        }
        /// <summary>
        /// 从数组中分析出ObjectId
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        public static bool TryParseObjectId(this object[] objects, out ObjectId[] objectIds)
        {
            objectIds = null;
            if (objects.All(p => p.ToString().IsObjectId()))
            {
                objectIds = objects.Select(p => ObjectId.Parse(p.ToString())).ToArray();
                return true;
            }

            return false;
        }
        /// <summary>
        /// 从指定的字符串中分析出ObjectId
        /// </summary>
        /// <param name="source"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool TryExtractObjectIdArray(this string source, out string[] array)
        {
            var arr = source.Split(',');
            var lst = new List<string>();

            if (arr.All(p => p.IsObjectId()) || source.IsObjectId())
            {
                lst.AddRange(arr.Where(s => s.IsObjectId()));
            }

            array = lst.ToArray();
            return lst.Count > 0;
        }
    }
}