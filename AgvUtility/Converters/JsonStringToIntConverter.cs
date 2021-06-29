using System;
using Newtonsoft.Json;
using Utility.Extensions;

namespace Utility.Converters
{
    /// <summary>
    /// 字符串转数字
    /// </summary>
    public class JsonStringToIntConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToStringEx();
            return int.TryParse(value, out var v) ? v : 0;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}