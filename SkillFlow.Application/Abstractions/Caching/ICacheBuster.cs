namespace SkillFlow.Application.Abstractions.Caching
{
    public interface ICacheBuster
    {
        int CurrentVersion { get; }
        void Bump();
    }
}
