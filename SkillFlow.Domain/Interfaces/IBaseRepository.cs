using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Domain.Interfaces
{
    public interface IBaseRepository<T, TId>
    {
        Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);        
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, byte[]? rowVersion, CancellationToken ct = default);
        Task<bool> DeleteAsync(TId id, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
        Task<PagedResult<T>> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            CancellationToken ct = default);
    }
}
