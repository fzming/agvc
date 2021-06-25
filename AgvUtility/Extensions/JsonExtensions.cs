using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Utility.Extensions
{
    public static class JsonExtensions
    {
        public static T As<T>(this JObject jObject)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(jObject));
        }

        public static List<T> ToList<T>(this JArray jArray)
        {
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(jArray));
        }
    }
}