using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utility
{
    public  static class Extension
    {
        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            if (source is string || source.GetType().IsValueType)
            {
                return source.ToString();
            }

            //序列化JSON处理ISO时间问题
            //Newtonsoft.Json产生的默认日期时间格式为： IsoDateTimeConverter 格式

            var dateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.SerializeObject(source, dateTimeConverter);
        }
 
        public static int ToInt(this string data)
        {
            var success = int.TryParse(data, out var result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt32(data, 0);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}