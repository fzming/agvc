using System;
using Newtonsoft.Json;
using Utility.Extensions;

namespace Utility.Converters
{
    /// <summary>
    /// UTC 转换成时间
    /// </summary>
    public class JsonUtcDateTimeConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToStringEx();
            var t= long.TryParse(value, out var v) ? v : 0;
            if (t>0)
            {
                return t.ToStringEx().ToUnixDatetime();
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}