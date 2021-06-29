using System;
using Newtonsoft.Json;
using Utility.Extensions;

namespace Utility.Converters
{
    /// <summary>
    /// 车辆里程转换
    /// </summary>
    public class JsonMlgConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToStringEx();
            if (int.TryParse(value, out var mlg))
            {
                return Convert.ToInt32(mlg/10);
            }

            return 0;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}