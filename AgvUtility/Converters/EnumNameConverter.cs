using System;
using Newtonsoft.Json;
using Utility.Extensions;

namespace Utility.Converters
{
    public class EnumNameConverter : JsonConverter
    {
        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var @enum = (Enum)value;

                var aliasName = @enum.GetAliasName();
                if (aliasName != null)
                {
                    writer.WriteValue(aliasName);
                }
                else
                    writer.WriteValue(value);
            }
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!objectType.IsEnum)
            {
                var underlyingType = Nullable.GetUnderlyingType(objectType);
                if (underlyingType != null && underlyingType.IsEnum)
                    objectType = underlyingType;
            }

            var value = reader.Value;

            string strValue;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                if (existingValue == null || Nullable.GetUnderlyingType(existingValue.GetType()) != null)
                    return null;
                strValue = "0";
            }
            else
                strValue = value.ToString();

            return int.TryParse(strValue, out var intValue) ?
                Enum.ToObject(objectType, intValue) :
                Enum.Parse(objectType, strValue);
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}