using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Utility.Attributes;

namespace Utility.Extensions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class ObjectExtensions
    {
        /// <summary>
        /// 比较两个对象的属性值异同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newObject">新对象</param>
        /// <param name="oldObject">旧对象</param>
        /// <param name="onlyHasTrackAttr">是否只考察包含TrackFieldAttribute特性的属性</param>
        /// <returns></returns>
        public static List<ChangedProperty> CompareProperties<T>(this T newObject, T oldObject, bool onlyHasTrackAttr = true) where T : class
        {
            var list = new List<ChangedProperty>();
            InternalCompareProperties(newObject, oldObject, typeof(T), ref list, onlyHasTrackAttr);
            return list;
        }


        private static void InternalCompareProperties(object newObject, object oldObject, Type t,
            ref List<ChangedProperty> compareResults, bool onlyHasTrackAttr,
            string parentPropertyName = "", string parentTrackName = "")
        {
            //为空判断
            if (newObject == null || oldObject == null)
                return;
            var props = t.GetProperties();
            foreach (var po in props)
            {
                var propertyName = po.Name;
                var parentPath = (parentPropertyName + ".").TrimStart('.');
                var parentPathName = (parentTrackName + ">").TrimStart('>');
                //只可比较值类型和string类型
                var canCompare = po.PropertyType.IsValueType || po.PropertyType == typeof(string);
                var oValue = po.GetValue(oldObject);
                var nValue = po.GetValue(newObject);
                if (oValue == null && nValue == null) continue;
                var trackAttr = po.GetCustomAttribute<TrackFieldAttribute>();

                if (canCompare)  //valueType
                {

                    if (trackAttr == null && onlyHasTrackAttr) continue;
                    if (oValue != null && nValue != null && nValue.Equals(oValue)) continue; //相同值
                    var originalValue = oValue.ToStringEx();
                    var newValue = nValue.ToStringEx();
                    
                    //enum
                    if (trackAttr?.ValueConverter != null)
                    {
                        var instance = Activator.CreateInstance(trackAttr.ValueConverter) as ITrackFieldValueConverter;
                        originalValue = instance.Convert(oValue, po.PropertyType);
                        newValue = instance.Convert(nValue, po.PropertyType);

                    }
                    if (originalValue.Trim() == newValue.Trim()) continue; //字符串值相同
                    compareResults.Add(new ChangedProperty
                    {
                        Field = propertyName,
                        Name = parentPathName + trackAttr?.Name ?? propertyName,
                        Path = parentPath + propertyName,
                        OriginalValue = originalValue,
                        NewValue = newValue
                    });

                }
                else if (po.PropertyType.IsEnumerableType()) //IList
                {
                    var arrNewValues = ((IEnumerable<object>)nValue).ToList();
                    var arrOldValues = ((IEnumerable<object>)oValue).ToList();
                    for (var i = 0; i < arrNewValues.Count; i++)
                    {
                        InternalCompareProperties(arrNewValues[i], arrOldValues.TryGetValue(i), arrNewValues[i].GetType(),
                            ref compareResults, onlyHasTrackAttr, parentPath + $"${i}." + propertyName);
                    }
                }
                else //object 比较的是对象，需记住父对象名称
                {

                    InternalCompareProperties(nValue, oValue, po.PropertyType,
                        ref compareResults, onlyHasTrackAttr, parentPath + propertyName, parentPathName + trackAttr?.Name);
                }
            }
        }

        /// <summary>
        /// 将IDictionary转成指定对象T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    ?.SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static object[] ToObjects<T>(this T[] values) where T : struct
        {
            return values.Cast<object>().ToArray();
        }

        /// <summary>
        /// 将指定的对象转成IDictionary
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bindingAttr"></param>
        /// <returns></returns>
        public static IDictionary<string, T> ToDictionary<T>(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            if (source == null)
            {
                return new ConcurrentDictionary<string, T>();
            }
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => (T)Convert.ChangeType(propInfo.GetValue(source, null), typeof(T))
            );

        }

        public static Uri AddQueryString(this Uri uri, object s)
        {
            var ub = new UriBuilder(uri) { Query = s.ToQueryString() };
            return ub.Uri;
        }
        public static Uri AddQueryString(this Uri uri, string name, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri) { Query = httpValueCollection.ToString() };

            return ub.Uri;
        }

        ///<summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        public static NameValueCollection ParseQueryString(this string url)
        {
            var uri = new Uri(url);
            return HttpUtility.ParseQueryString(uri.Query, Encoding.UTF8);
        }
        /// <summary>
        /// 将Object转换成查询字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="autoUrlEncode">是否自动进行UrlEncode进行编码</param>
        /// <returns></returns>
        public static string ToQueryString(this object source, bool autoUrlEncode = true)
        {
            if (source == null)
            {
                return string.Empty;
            }

            var dic = source.ToDictionary<string>();
            return autoUrlEncode
                ? dic.Select(p => $"{p.Key}={p.Value.UrlEncode()}").JoinToString("&")
                : dic.Select(p => $"{p.Key}={p.Value}").JoinToString("&");
        }

        public static FormUrlEncodedContent ToFormUrlEncodedContent(this object source)
        {
            if (source == null)
            {
                return null;
            }
            var dic = source.ToDictionary<string>();
            return new FormUrlEncodedContent(dic);
        }
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
        public static object GetValue(this MemberInfo member, object obj)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.GetValue(obj);
            var property = member as PropertyInfo;
            if (property != null)
                return property.GetValue(obj);

            throw new NotSupportedException($"The type '{member.GetType()}' is not supported.");
        }
        public static Type GetReturnType(this MethodBase method)
        {
            var returnType = (method as MethodInfo)?.ReturnType;

            //泛型返回类型
            if (returnType != null &&
                returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return returnType.GetGenericArguments()[0];
            }
            return returnType;
        }
    }

}