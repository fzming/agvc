using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoreRepository.Converters
{
    /// <summary>
    ///     序列化时：将iso日期标准的日期字段 转换成 中国标准时间
    /// </summary>
    /// <example>
    ///     [JsonConverter(typeof(IsoDateTimeConverter))]
    ///     public DateTime Birthday { get; set; }
    /// </example>
    internal sealed class ChinaDateTimeConverter : DateTimeConverterBase
    {
        private static readonly IsoDateTimeConverter DtConverter = new() {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"};

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return DtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DtConverter.WriteJson(writer, value, serializer);
        }
    }
}