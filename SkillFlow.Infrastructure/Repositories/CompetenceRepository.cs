using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Infrastructure.Repositories
{
    public class CompetenceRepository(SkillFlowDbContext context)
        : BaseRespository<Competence, CompetenceId>(context),
        ICompetenceRepository
    {
        public async Task<bool> ExistsByNameAsync(CompetenceName name, CancellationToken ct = default)
            => await _context.Competences.AnyAsync(c => c.Name == name, ct);
        

        public async Task<Competence?> GetByNameAsync(CompetenceName name, CancellationToken ct = default)
            => await _context.Competences.FirstOrDefaultAsync(c => c.Name == name, ct);

        public override async Task<Competence?> GetByIdAsync(CompetenceId id, CancellationToken ct = default)
        {
            return await _context.Competences
                .Include(c => c.Instructors)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public override async Task<IEnumerable<Competence>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Competences
                .Include(c => c.Instructors)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<PagedResult<Competence>> GetPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {
          Expression<Func<Competence, bool>>? filter = null;

          if(!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                filter = c => EF.Functions.Like(c.Name.Value, $"{term}");
            }

            return await GetPagedAsync(page, pageSize, filter, ct: ct);
        }
    }

}
