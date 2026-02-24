using Microsoft.Extensions.Caching.Memory;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Infrastructure.Caching
{
    public sealed class CachedAttendeeRepository : IAttendeeRepository
    {
        private readonly IAttendeeRepository _inner;
        private readonly IMemoryCache _cache;
        private readonly IAttendeeCacheBuster _buster;

        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(2);

        public CachedAttendeeRepository(
            IAttendeeRepository inner,
            IMemoryCache cache,
            IAttendeeCacheBuster buster)
        {
            _inner = inner;
            _cache = cache;
            _buster = buster;
        }

        private string V(string key) => CacheKey.V(_buster.CurrentVersion, key);


        public Task<Attendee?> GetByEmailAsync(Email email, CancellationToken ct = default)
        {
            var key = V($"attendee:email:{email.Value.Trim().ToLowerInvariant()}");
            return _cache.GetOrCreateAsync(key, DefaultTtl, () => _inner.GetByEmailAsync(email, ct));
        }

        public Task<Attendee?> GetByIdAsync(AttendeeId id, CancellationToken ct = default)
        {
            var key = V($"attendee:id:{id.Value}");
            return _cache.GetOrCreateAsync(key, DefaultTtl, () => _inner.GetByIdAsync(id, ct));
        }

        public Task AddAsync(Attendee entity, CancellationToken ct = default) => _inner.AddAsync(entity, ct);
        public Task<bool> DeleteAsync(AttendeeId id, CancellationToken ct = default) => _inner.DeleteAsync(id, ct);
        public Task UpdateAsync(Attendee entity, byte[]? rowVersion, CancellationToken ct = default) => _inner.UpdateAsync(entity, rowVersion, ct);

        public Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default) => _inner.ExistsByEmailAsync(email, ct);

        public Task<bool> ExistsByIdAsync(AttendeeId id, CancellationToken ct = default) => _inner.ExistsByIdAsync(id, ct);

        public Task<IEnumerable<Attendee>> GetAllAsync(CancellationToken ct = default) => _inner.GetAllAsync(ct);
        public Task<List<Instructor>> GetInstructorsByIdsAsync(IReadOnlyCollection<AttendeeId> ids, CancellationToken ct) =>
            _inner.GetInstructorsByIdsAsync(ids, ct);

        public Task<PagedResult<Attendee>> GetPagedAsync(int page, int pageSize, Expression<Func<Attendee, bool>>? filter = null, Func<IQueryable<Attendee>, IQueryable<Attendee>>? include = null, CancellationToken ct = default) =>
            _inner.GetPagedAsync(page, pageSize, filter, include, ct);
    }
}
