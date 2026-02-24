namespace SkillFlow.Application.Abstractions.Caching
{
    public interface IAttendeeCacheBuster
    {
        int CurrentVersion { get; }
        void Bump();
    }
}
