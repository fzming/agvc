using System;
using Newtonsoft.Json;
using Utility.Extensions;

namespace Utility.Converters
{
    /// <summary>
    /// 经纬度转换
    /// </summary>
    public class JsonLngLatConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToStringEx();
            if (int.TryParse(value, out var v))
            {
                if (v>1000)
                {
                    return v / 600000.0;
                }

                return v;
            }

            return 0.0D;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}