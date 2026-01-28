namespace SkillFlow.Domain.Primitives
{
    public static class StringExtensions
    {
        public static string NormalizeName(this string value) => value.Trim();
    }
}
