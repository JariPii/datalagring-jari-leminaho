using Microsoft.Extensions.Caching.Memory;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Infrastructure.Caching
{
    public sealed class CachedAttendeeQueries : IAttendeeQueries
    {
        private readonly IAttendeeQueries _inner;
        private readonly IMemoryCache _cache;
        private readonly IAttendeeCacheBuster _buster;

        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan ShortTtl = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan LongTtl = TimeSpan.FromMinutes(10);

        public CachedAttendeeQueries(
            IAttendeeQueries inner,
            IMemoryCache cache,
            IAttendeeCacheBuster buster)
        {
            _inner = inner;
            _cache = cache;
            _buster = buster;
        }

        private string V(string key) => CacheKey.V(_buster.CurrentVersion, key);

        public Task<IEnumerable<Attendee>> GetAllAsync(CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V("attendees:all"), DefaultTtl, () => _inner.GetAllAsync(ct));

        public Task<IEnumerable<Instructor>> GetAllInstructorsAsync(CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V("attendees:instructors:all"), DefaultTtl, () => _inner.GetAllInstructorsAsync(ct));

        public Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V("attendees:students:all"), DefaultTtl, () => _inner.GetAllStudentsAsync(ct));

        public Task<Competence?> GetCompetenceByNameAsync(CompetenceName name, CancellationToken ct = default)
        {
            var key = V($"competence:{name.Value}");
            return _cache.GetOrCreateAsync(key, LongTtl, () => _inner.GetCompetenceByNameAsync(name, ct));
        }

        public Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName, CancellationToken ct = default)
        {
            var key = V($"attendees:instructors:competence:{CacheKey.Normalize(competenceName)}");
            return _cache.GetOrCreateAsync(key, DefaultTtl, () => _inner.GetInstructorsByCompetenceAsync(competenceName, ct));
        }

        public Task<PagedResult<Instructor>> GetInstructorsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {
            var key = V($"instructors:paged:p{page}:s{pageSize}:q{CacheKey.Normalize(q)}");
            return _cache.GetOrCreateAsync(key, DefaultTtl, () => _inner.GetInstructorsPagedAsync(page, pageSize, q, ct));
        }

        public Task<PagedResult<Student>> GetStudentsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {
            var key = V($"students:paged:p{page}:s{pageSize}:q:{CacheKey.Normalize(q)}");
            return _cache.GetOrCreateAsync(key, ShortTtl, () => _inner.GetStudentsPagedAsync(page, pageSize, q, ct));
        }

        public Task<IEnumerable<Attendee>> SearchByNameAsync(string searchTerm, CancellationToken ct = default)
        {
            var key = V($"attendees:search:{CacheKey.Normalize(searchTerm)}");
            return _cache.GetOrCreateAsync(key, ShortTtl, () => _inner.SearchByNameAsync(searchTerm, ct));
        }
    }
}
