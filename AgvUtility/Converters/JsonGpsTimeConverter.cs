using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Utility.Extensions;

namespace Utility.Converters
{
    /// <summary>
    /// GPS时间转换
    /// </summary>
    public class JsonGpsTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToStringEx();
            if (value.IsNullOrEmpty()) return null;
            var t =  Regex.Replace(value, @"(\d{4})(\d{2})(\d{2})\/(\d{2})(\d{2})(\d{2})", "$1-$2-$3 $4:$5:$6", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            if (DateTime.TryParse(t, out var dt))
            {
                return dt;
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}