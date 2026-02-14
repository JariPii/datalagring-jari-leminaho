using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Infrastructure.Repositories
{
    public abstract class BaseRespository<T, TId>(SkillFlowDbContext context)
        where T : BaseEntity<TId>, IAggregateRoot
    {
        protected readonly SkillFlowDbContext _context = context;

        public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await _context.Set<T>().AddAsync(entity, ct);
        }

        public virtual Task UpdateAsync(T entity, byte[] rowVersion, CancellationToken ct = default)
        {
            _context.Entry(entity).Property("RowVersion").OriginalValue = rowVersion;

            if (_context.Entry(entity).State == EntityState.Detached)
                _context.Attach(entity);

            return Task.CompletedTask;
        }

        public virtual async Task<bool> DeleteAsync(TId id, CancellationToken ct = default)
        {
            var entity = await _context.Set<T>().FindAsync([id], ct);

            if (entity is null) return false;

            _context.Set<T>().Remove(entity);
            return true;
        }

        public virtual async Task<T?> GetByIdAsync(TId id, CancellationToken ct = default) =>
            await _context.Set<T>().FindAsync([id], ct);
        

        public virtual async Task<bool> ExistsByIdAsync(TId id, CancellationToken ct = default)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id!.Equals(id), ct);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
