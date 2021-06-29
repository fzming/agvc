using System;
using Newtonsoft.Json;
using Utility.Extensions;

namespace Utility.Converters
{
    /// <summary>
    /// GPS速度转换
    /// </summary>
    public class JsonGpsSpeedConverter:JsonConverter
    {
        private int Divisor { get; }

        public JsonGpsSpeedConverter(int divisor=10)
        {
            Divisor = divisor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToStringEx();
            if (double.TryParse(value, out var spd))
            {
                if (Divisor == 0) return spd;
                return spd / Divisor;
            }

            return 0.0D;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}