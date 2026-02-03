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
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public virtual async Task<bool> DeleteAsync(TId id, CancellationToken ct = default)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);

            if (entity is null) return false;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public virtual async Task<T?> GetByIdAsync(TId id, CancellationToken ct = default)
            => await _context.Set<T>().FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);
        

        public virtual async Task<bool> ExistsByIdAsync(TId id, CancellationToken ct = default)
        {
            return await _context.Set<T>().AnyAsync(e => e.Id!.Equals(id), ct);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
