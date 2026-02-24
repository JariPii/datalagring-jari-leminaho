using Microsoft.Extensions.Caching.Memory;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Infrastructure.Caching
{
    public sealed class CachedCourseRepository : ICourseRepository
    {
        private readonly ICourseRepository _inner;
        private readonly IMemoryCache _cache;
        private readonly ICourseCacheBuster _buster;

        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan ShortTtl = TimeSpan.FromSeconds(30);

        public CachedCourseRepository(ICourseRepository inner, IMemoryCache cache, ICourseCacheBuster buster)
        {
            _inner = inner;
            _cache = cache;
            _buster = buster;
        }

        public string V(string key) => CacheKey.V(_buster.CurrentVersion, key);

        public Task<Course?> GetByCourseNameAsync(CourseName name, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"course:name:{CacheKey.Normalize(name.Value)}"), 
            DefaultTtl, () => _inner.GetByCourseNameAsync(name, ct));

        public Task<Course?> GetByCourseCodeAsync(CourseCode code, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"course:code:{CacheKey.Normalize(code.Value)}"),
                DefaultTtl, () => _inner.GetByCourseCodeAsync(code, ct));

        public Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"course:search:{CacheKey.Normalize(searchTerm)}"),
                ShortTtl, () => _inner.SearchByNameAsync(searchTerm, ct));
        public Task<PagedResult<Course>> GetCoursePagedAsync(int page, int pageSize, string? q, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"course:paged:p{page}:s{pageSize}:q{CacheKey.Normalize(q)}"),
                ShortTtl, () => _inner.GetCoursePagedAsync(page, pageSize, q, ct));

        public Task<bool> ExistsByCourseName(CourseName name, CancellationToken ct = default) =>
            _inner.ExistsByCourseName(name, ct);

        public Task<bool> ExistsByCourseCodeAsync(CourseCode code, CancellationToken ct = default) =>
            _inner.ExistsByCourseCodeAsync(code, ct);

        public Task<int> GetMaxSuffixAsync(string coursePart, CourseType type, CancellationToken ct = default) =>
            _inner.GetMaxSuffixAsync(coursePart, type, ct);

        public Task<bool> IsCourseInUseAsync(CourseId id, CancellationToken ct = default) =>
            _inner.IsCourseInUseAsync(id, ct);

        public Task AddAsync(Course entity, CancellationToken ct = default) =>
            _inner.AddAsync(entity, ct);

        public Task<bool> DeleteAsync(CourseId id, CancellationToken ct = default) =>
            _inner.DeleteAsync(id, ct);

        public Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default) =>
            _inner.GetAllAsync(ct);

        public Task<Course?> GetByIdAsync(CourseId id, CancellationToken ct = default) =>
            _inner.GetByIdAsync(id, ct);

        public void Remove(Course course) => _inner.Remove(course);

        public Task UpdateAsync(Course entity, byte[]? rowVersion, CancellationToken ct = default) =>
            _inner.UpdateAsync(entity, rowVersion, ct);

        public Task<PagedResult<Course>> GetPagedAsync(int page, int pageSize, Expression<Func<Course, bool>>? filter = null, Func<IQueryable<Course>, IQueryable<Course>>? include = null, CancellationToken ct = default) =>
            _inner.GetPagedAsync(page, pageSize, filter, include, ct);
    }
}
