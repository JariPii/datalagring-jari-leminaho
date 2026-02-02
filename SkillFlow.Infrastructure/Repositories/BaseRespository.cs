using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Repositories
{
    public abstract class BaseRespository<T, TId>(SkillFlowDbContext context)
        where T : BaseEntity<TId>, IAggregateRoot
    {
        public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        {
            await context.Set<T>().AddAsync(entity, ct);
            await context.SaveChangesAsync(ct);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync(ct);
        }

        public virtual async Task<bool> DeleteAsync(TId id, CancellationToken ct = default)
        {
            var entity = await context.Set<T>().FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);

            if (entity is null) return false;

            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync(ct);
            return true;
        }

        public virtual async Task<bool> ExistsByIdAsync(TId id, CancellationToken ct = default)
        {
            return await context.Set<T>().AnyAsync(e => e.Id!.Equals(id), ct);
        }
    }
}
