using System;
using MongoDB.Bson.Serialization;

namespace CoreRepository.Converters
{
    public class CustomDoubleSerializer : IBsonSerializer
    {
        /// <summary>Deserializes a value.</summary>
        /// <param name="context">The deserialization context.</param>
        /// <param name="args">The deserialization args.</param>
        /// <returns>A deserialized value.</returns>
        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            var rep = bsonReader.ReadDouble();
            return rep;
        }

        /// <summary>Serializes a value.</summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="args">The serialization args.</param>
        /// <param name="value">The value.</param>
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var bsonWriter = context.Writer;
            var rep = (decimal) (double) value;
            bsonWriter.WriteDecimal128(rep);
        }

        /// <summary>Gets the type of the value.</summary>
        /// <value>The type of the value.</value>
        public Type ValueType => typeof(double);
    }
}