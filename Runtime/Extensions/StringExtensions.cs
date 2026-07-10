using System.Text;

namespace Dreamy.Core
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        public static string OrEmpty(this string value) => value ?? string.Empty;

        public static string ToPascalCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length);
            var uppercaseNext = true;
            for (var i = 0; i < value.Length; i++)
            {
                var character = value[i];
                if (!char.IsLetterOrDigit(character))
                {
                    uppercaseNext = true;
                    continue;
                }

                builder.Append(uppercaseNext ? char.ToUpperInvariant(character) : char.ToLowerInvariant(character));
                uppercaseNext = false;
            }

            return builder.ToString();
        }

        public static string ToSnakeCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length + 8);
            for (var i = 0; i < value.Length; i++)
            {
                var character = value[i];
                if (!char.IsLetterOrDigit(character))
                {
                    if (builder.Length > 0 && builder[builder.Length - 1] != '_')
                    {
                        builder.Append('_');
                    }

                    continue;
                }

                if (char.IsUpper(character) && builder.Length > 0 && builder[builder.Length - 1] != '_')
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(character));
            }

            return builder.ToString().Trim('_');
        }
    }
}
