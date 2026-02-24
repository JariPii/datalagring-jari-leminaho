using SkillFlow.Application.Abstractions.Caching;

namespace SkillFlow.Infrastructure.Caching
{
    public sealed class AttendeeCacheBuster : MemoryCacheBuster, IAttendeeCacheBuster { }
    public sealed class CourseCacheBuster : MemoryCacheBuster, ICourseCacheBuster { }
    public sealed class LocationCacheBuster : MemoryCacheBuster, ILocationCacheBuster { }
    public sealed class CompetenceCacheBuster : MemoryCacheBuster, ICompetenceCacheBuster { }
    public sealed class CourseSessionCacheBuster : MemoryCacheBuster, ICourseSessionCacheBuster { }
}
