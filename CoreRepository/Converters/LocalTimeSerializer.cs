using System;
using MongoDB.Bson.Serialization;

namespace CoreRepository.Converters
{
    internal sealed class LocalTimeSerializer : IBsonSerializer
    {
        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return context.Reader.ReadDateTime();
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            context.Writer.WriteDateTime(((DateTime) value).ToLocalTime().Ticks);
        }

        public Type ValueType => typeof(DateTime);
    }
}