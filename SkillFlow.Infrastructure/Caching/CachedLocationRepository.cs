using Microsoft.Extensions.Caching.Memory;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;
using System.Numerics;

namespace SkillFlow.Infrastructure.Caching
{
    public sealed class CachedLocationRepository : ILocationRepository
    {
        private readonly ILocationRepository _inner;
        private readonly IMemoryCache _cache;
        private readonly ILocationCacheBuster _buster;

        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan ShortTtl = TimeSpan.FromSeconds(30);

        public CachedLocationRepository(ILocationRepository inner, IMemoryCache cache, ILocationCacheBuster buster)
        {
            _inner = inner;
            _cache = cache;
            _buster = buster;
        }

        private string V(string key) => CacheKey.V(_buster.CurrentVersion, key);

        public Task AddAsync(Location entity, CancellationToken ct = default) =>
            _inner.AddAsync(entity, ct);

        public Task<bool> DeleteAsync(LocationId id, CancellationToken ct = default) =>
            _inner.DeleteAsync(id, ct);

        public Task<bool> ExistsByIdAsync(LocationId id, CancellationToken ct = default) =>
            _inner.ExistsByIdAsync(id, ct);

        public Task<bool> ExistsByNameAsync(LocationName name, CancellationToken ct = default) =>
            _inner.ExistsByNameAsync(name, ct);

        public Task<IEnumerable<Location>> GetAllAsync(CancellationToken ct = default) =>
            _inner.GetAllAsync(ct);

        public Task<Location?> GetByIdAsync(LocationId id, CancellationToken ct = default) =>
            _inner.GetByIdAsync(id, ct);

        public Task<Location?> GetByLocationNameAsync(LocationName name, CancellationToken ct) =>
            _cache.GetOrCreateAsync(V($"location:name:{CacheKey.Normalize(name.Value)}"),
                DefaultTtl, () => _inner.GetByLocationNameAsync(name, ct));

        public Task<PagedResult<Location>> GetLocationsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"location:paged:p{page}:s{pageSize}:q{CacheKey.Normalize(q)}"),
                ShortTtl, () => _inner.GetLocationsPagedAsync(page, pageSize, q, ct));

        public Task<PagedResult<Location>> GetPagedAsync(int page, int pageSize, Expression<Func<Location, bool>>? filter = null, Func<IQueryable<Location>, IQueryable<Location>>? include = null, CancellationToken ct = default) =>
            _inner.GetPagedAsync(page, pageSize, filter, include, ct);

        public Task<bool> IsLocationInUseAsync(LocationId id, CancellationToken ct = default) =>
            _inner.IsLocationInUseAsync(id, ct);

        public void Remove(Location location)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Location>> SearchByNameAsync(string searchTerm, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"location:search{CacheKey.Normalize(searchTerm)}"),
                ShortTtl, () => _inner.SearchByNameAsync(searchTerm, ct));

        public Task UpdateAsync(Location entity, byte[]? rowVersion, CancellationToken ct = default) =>
            _inner.UpdateAsync(entity, rowVersion, ct);
    }
}
