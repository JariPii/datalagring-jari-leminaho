using Microsoft.Extensions.Caching.Memory;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Infrastructure.Caching
{
    public sealed class CachedCourseSessionRepository : ICourseSessionRepository
    {
        private readonly ICourseSessionRepository _inner;
        private readonly IMemoryCache _cache;
        private readonly ICourseSessionCacheBuster _buster;

        private static readonly TimeSpan DefaultTtl = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ShortTtl = TimeSpan.FromSeconds(20);
        private static readonly TimeSpan LongTtl = TimeSpan.FromMinutes(2);

        public CachedCourseSessionRepository(ICourseSessionRepository inner, IMemoryCache cache, ICourseSessionCacheBuster buster)
        {
            _inner = inner;
            _cache = cache;
            _buster = buster;
        }

        private string V(string key) => CacheKey.V(_buster.CurrentVersion, key);

        public Task AddAsync(CourseSession entity, CancellationToken ct = default) =>
            _inner.AddAsync(entity, ct);

        public Task<bool> DeleteAsync(CourseSessionId id, CancellationToken ct = default) =>
            _inner.DeleteAsync(id, ct);

        public Task<bool> ExistsByIdAsync(CourseSessionId id, CancellationToken ct = default) =>
            _inner.ExistsByIdAsync(id, ct);

        public Task<IEnumerable<CourseSession>> GetAllAsync(CancellationToken ct = default) =>
            _inner.GetAllAsync(ct);

        public Task<PagedResult<CourseSession>> GetPagedAsync(int page, int pageSize, Expression<Func<CourseSession, bool>>? filter = null, Func<IQueryable<CourseSession>, IQueryable<CourseSession>>? include = null, CancellationToken ct = default) =>
            _inner.GetPagedAsync(page, pageSize, filter, include, ct);

        public Task UpdateAsync(CourseSession entity, byte[]? rowVersion, CancellationToken ct = default) =>
            _inner.UpdateAsync(entity, rowVersion, ct);

        public Task<CourseSession?> GetByIdAsync(CourseSessionId id, CancellationToken ct = default) =>
            _inner.GetByIdAsync(id, ct);

        public Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:courseCode:{CacheKey.Normalize(code.Value)}"),
                DefaultTtl,
                () => _inner.GetByCourseCodeAsync(code, ct));

        public Task<CourseSession?> GetByIdWithInstructorsAndEnrollmentsAsync(CourseSessionId id, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:id:full:{id.Value}"),
                ShortTtl,
                () => _inner.GetByIdWithInstructorsAndEnrollmentsAsync(id, ct));

        public Task<IEnumerable<CourseSession>> GetByLocationAsync(LocationId locationId, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:location:{locationId.Value}"),
                DefaultTtl,
                () => _inner.GetByLocationAsync(locationId, ct));

        public Task<PagedResult<CourseSession>> GetCourseSessionsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:paged:p{page}:s{pageSize}:q{CacheKey.Normalize(q)}"),
                DefaultTtl,
                () => _inner.GetCourseSessionsPagedAsync(page, pageSize, q, ct));

        public Task<IEnumerable<CourseSession>> GetSessionInDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:range:{startDate:yyyyMMdd}-{endDate:yyyyMMdd}"),
                DefaultTtl,
                () => _inner.GetSessionInDateRangeAsync(startDate, endDate, ct));

        public Task<IEnumerable<CourseSession>> GetSessionsWithAvailableCapacityAsync(CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:available"),
                ShortTtl,
                () => _inner.GetSessionsWithAvailableCapacityAsync(ct));

        public Task<IEnumerable<CourseSession>> SearchAsync(string searchTerm, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:search:{CacheKey.Normalize(searchTerm)}"),
                ShortTtl,
                () => _inner.SearchAsync(searchTerm, ct));

        public Task<IEnumerable<CourseSession>> SearchByEndDateAsync(DateTime endDate, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:end:{endDate:yyyyMMdd}"),
                DefaultTtl,
                () => _inner.SearchByEndDateAsync(endDate, ct));

        public Task<IEnumerable<CourseSession>> SearchByStartDateAsync(DateTime startDate, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"session:start:{startDate:yyyyMMdd}"),
                DefaultTtl,
                () => _inner.SearchByStartDateAsync(startDate, ct));

    }
}
