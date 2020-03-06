namespace Evolution.Serialisation
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    sealed class SlugCaseEnumConverter :
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

                string parsed = ToPascalCase(enumText);
                if (Enum.TryParse(t, parsed, true, out var result))
                    return result;

                if (Enum.TryParse(t, enumText, true, out result))
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
            string pascalCase = e.ToString();
            if (!string.IsNullOrEmpty(pascalCase))
            {
                string slugCase = ToSlugCase(pascalCase);
                writer.WriteValue(slugCase);
                return;
            }

            writer.WriteValue(value);
        }

        static Type GetEnumType(Type input) =>
            (input.IsGenericType && input.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Nullable.GetUnderlyingType(input) : input;

        public static string ToSlugCase(string pascalCase) => new string(ToSlugCaseLoop(pascalCase).ToArray());

        static IEnumerable<char> ToSlugCaseLoop(string pascalCase)
        {
            var chars = pascalCase.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                yield return char.ToLowerInvariant(c);

                // End of string
                if (i + 1 >= chars.Length)
                    continue;

                char next = chars[i + 1];

                // Switching from alpha to numeric or vice versa
                if (char.IsNumber(c) != char.IsNumber(next))
                    yield return '-';
                // Switching from lower to upper
                else if (char.IsUpper(next) && char.IsLower(c))
                    yield return '-';
            }
        }

        static string ToPascalCase(string slugCase) => new string(ToPascalCaseLoop(slugCase).ToArray());

        static IEnumerable<char> ToPascalCaseLoop(string slugCase)
        {
            var chars = slugCase.ToCharArray();
            bool newWord = true;
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == '-')
                    newWord = true;
                else if (char.IsNumber(c))
                {
                    newWord = true;
                    yield return c;
                }
                else if (newWord)
                    yield return char.ToUpperInvariant(c);
                else
                    yield return char.ToLowerInvariant(c);
            }
        }
    }
}