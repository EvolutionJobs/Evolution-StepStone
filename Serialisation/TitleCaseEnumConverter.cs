namespace Evolution.Serialisation
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    sealed class TitleCaseEnumConverter :
        JsonConverter
    {
        public override bool CanConvert(Type objectType) => GetEnumType(objectType).IsEnum;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var t = GetEnumType(objectType);

            if (reader.TokenType == JsonToken.Integer)
                return Enum.ToObject(t, reader.Value);

            if (reader.TokenType == JsonToken.String)
            {
                string enumText = reader.Value.ToString();

                if (string.IsNullOrEmpty(enumText) ||
                    string.Equals("null", enumText, StringComparison.InvariantCultureIgnoreCase)) // Some enums appear to have a "null" string value
                    return null;

                if (Enum.TryParse(t, enumText, true, out var result))
                    return result;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var e = (Enum)value;
            string titleCase = e.ToString();
            if (string.IsNullOrEmpty(titleCase))
                writer.WriteValue(value);
            else
                writer.WriteValue(titleCase);
        }

        static Type GetEnumType(Type input) =>
            (input.IsGenericType && input.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Nullable.GetUnderlyingType(input) : input;
    }
}