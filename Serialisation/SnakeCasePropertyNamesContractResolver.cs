namespace Evolution.Serialisation
{
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Resolve snake_named_properties in JSON.</summary>
    sealed class SnakeCasePropertyNamesContractResolver : 
        DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName) => new string(ToSnakeCaseLoop(propertyName).ToArray());

        static IEnumerable<char> ToSnakeCaseLoop(string pascalCase)
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
                    yield return '_';
                // Switching from lower to upper
                else if (char.IsUpper(next) && char.IsLower(c))
                    yield return '_';
            }
        }
    }
}