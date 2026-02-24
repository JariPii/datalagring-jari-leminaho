namespace SkillFlow.Application.Abstractions.Caching
{
    public sealed class MemoryAttendeeCacheBuster : IAttendeeCacheBuster
    {
        private int _version = 1;
        public int CurrentVersion => Volatile.Read(ref _version);
        public void Bump() => Interlocked.Increment(ref _version);
    }
}
