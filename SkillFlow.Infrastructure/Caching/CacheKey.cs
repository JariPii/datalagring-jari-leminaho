namespace SkillFlow.Infrastructure.Caching
{
    public static class CacheKey
    {
        public static string V(int version, string key) => $"v{version}:{key}";

        public static string Normalize(string? s) =>
            string.IsNullOrWhiteSpace(s) ? "-" : s.Trim().ToLowerInvariant();
    }
}
