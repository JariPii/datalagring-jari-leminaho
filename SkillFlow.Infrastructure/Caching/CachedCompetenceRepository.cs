using Microsoft.Extensions.Caching.Memory;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Infrastructure.Caching
{ 
    public sealed class CachedCompetenceRepository : ICompetenceRepository
    {
        private readonly ICompetenceRepository _inner;
        private readonly IMemoryCache _cache;
        private readonly ICompetenceCacheBuster _buster;

        private static readonly TimeSpan LongTtl = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan ShortTtl = TimeSpan.FromSeconds(30);

        public CachedCompetenceRepository(ICompetenceRepository inner, IMemoryCache cache, ICompetenceCacheBuster buster)
        {
            _inner = inner;
            _cache = cache;
            _buster = buster;
        }

        private string V(string key) => CacheKey.V(_buster.CurrentVersion, key);

        public Task AddAsync(Competence entity, CancellationToken ct = default) =>
            _inner.AddAsync(entity, ct);

        public Task<bool> DeleteAsync(CompetenceId id, CancellationToken ct = default) =>
            _inner.DeleteAsync(id, ct);

        public Task<bool> ExistsByNameAsync(CompetenceName name, CancellationToken ct = default) =>
            _inner.ExistsByNameAsync(name, ct);

        public Task<IEnumerable<Competence>> GetAllAsync(CancellationToken ct = default) =>
            _inner.GetAllAsync(ct);

        public Task<Competence?> GetByIdAsync(CompetenceId id, CancellationToken ct = default) =>
            _inner.GetByIdAsync(id, ct);

        public Task<Competence?> GetByNameAsync(CompetenceName name, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"competences:name:{CacheKey.Normalize(name.Value)}"),
                LongTtl, () => _inner.GetByNameAsync(name, ct));

        public Task<PagedResult<Competence>> GetPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default) =>
            _cache.GetOrCreateAsync(V($"competences:paged:p{page}:s{pageSize}:q{CacheKey.Normalize(q)}"),
                ShortTtl, () => _inner.GetPagedAsync(page, pageSize, q, ct));

        public Task<PagedResult<Competence>> GetPagedAsync(int page, int pageSize, Expression<Func<Competence, bool>>? filter = null, Func<IQueryable<Competence>, IQueryable<Competence>>? include = null, CancellationToken ct = default) =>
            _inner.GetPagedAsync(page, pageSize, filter, include,ct);

        public Task UpdateAsync(Competence entity, byte[]? rowVersion, CancellationToken ct = default) =>
            _inner.UpdateAsync(entity, rowVersion, ct);
    }
}
