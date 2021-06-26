using System;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace CoreRepository.Converters
{
    /**
     * [ScriptIgnore]//使用JavaScriptSerializer序列化时不序列化此字段  
     * [IgnoreDataMember]//使用DataContractJsonSerializer序列化时不序列化此字段  
     * [JsonIgnore]//使用JsonConvert序列化时不序列化此字段
     */
    internal sealed class BsonObjectIdConverter : JsonConverter
    {
        /// <summary>
        ///     序列化字段时
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        /// <summary>
        ///     反序列化字段时
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(reader.Value?.ToString())) return null;

            ObjectId.TryParse(reader.Value.ToString(), out var objectid);
            return objectid;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}