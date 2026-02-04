namespace SkillFlow.Domain.Interfaces
{
    public interface IBaseRepository<T, TId>
    {
        Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);        
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(TId id, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    }
}
