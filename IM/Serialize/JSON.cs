using System;
using System.IO;
using Newtonsoft.Json;

namespace Serialize
{
    public static class JSON
    {
        public static object DeserializeJsonToObject(this string json)
        {
            object obj2;
            try
            {
                var type = json.DeserializeJsonToObject<JsonType>();
                obj2 = json.DeserializeJsonToObject(type.Type);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("请先安装 Newtonsoft.Json");
            }
            catch (Exception)
            {
                obj2 = null;
            }

            return obj2;
        }

        public static T DeserializeJsonToObject<T>(this string json) where T : class
        {
            T local;
            try
            {
                var reader = new StringReader(json);
                local = new JsonSerializer().Deserialize(new JsonTextReader(reader), typeof(T)) as T;
            }
            catch (FileNotFoundException)
            {
                throw new Exception("请先安装 Newtonsoft.Json");
            }
            catch (Exception)
            {
                local = default;
            }

            return local;
        }

        public static object DeserializeJsonToObject(this string json, Type type)
        {
            object obj2;
            try
            {
                var reader = new StringReader(json);
                obj2 = new JsonSerializer().Deserialize(new JsonTextReader(reader), type);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("请先安装 Newtonsoft.Json");
            }
            catch (Exception)
            {
                obj2 = null;
            }

            return obj2;
        }

        public static T DeserializeJsonToObject<T>(this string json, T anonymous) where T : class
        {
            T local;
            try
            {
                var reader = new StringReader(json);
                local = new JsonSerializer().Deserialize(new JsonTextReader(reader), typeof(T)) as T;
            }
            catch (FileNotFoundException)
            {
                throw new Exception("请先安装 Newtonsoft.Json");
            }
            catch (Exception)
            {
                local = default;
            }

            return local;
        }

        public static string SerializeJSONObject(this object o)
        {
            string str;
            try
            {
                str = JsonConvert.SerializeObject(o);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("请先安装 Newtonsoft.Json");
            }

            return str;
        }

        public static string SerializeJSONObject(this object o, params string[] hidden)
        {
            string str;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new IgnorePropertiesResolver(hidden)
                };
                str = JsonConvert.SerializeObject(o, Formatting.None, settings);
            }
            catch (FileNotFoundException)
            {
                throw new Exception("请先安装 Newtonsoft.Json");
            }

            return str;
        }

        public abstract class AutoJsonable : IAutoJsonable
        {
            public Type Type => GetType();
        }

        public interface IAutoJsonable
        {
            Type Type { get; }
        }


        private class JsonType : IAutoJsonable
        {
            public Type Type { get; set; }
        }
    }
}