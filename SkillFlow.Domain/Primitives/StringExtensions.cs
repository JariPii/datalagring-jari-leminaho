using System.Globalization;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Primitives
{
    public static partial class StringExtensions
    {
        public static string NormalizeName(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var collapsed = WhitespaceRegex().Replace(value.Trim(), " ");

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(collapsed.ToLower());
        }


        public static string NormalizeText(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return WhitespaceRegex().Replace(value.Trim(), " ");
        }


        [GeneratedRegex(@"\s+")]
        private static partial Regex WhitespaceRegex();
    }
}
