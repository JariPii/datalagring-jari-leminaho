using SkillFlow.Application.Abstractions.Caching;

namespace SkillFlow.Infrastructure.Caching
{
    public class MemoryCacheBuster : ICacheBuster
    {
        private int _version = 1;
        public int CurrentVersion => Volatile.Read(ref _version);

        public void Bump() => Interlocked.Increment(ref _version);
    }
}
