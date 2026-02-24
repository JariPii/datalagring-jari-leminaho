using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICompetenceRepository : IBaseRepository<Competence, CompetenceId>
    {
        Task<Competence?> GetByNameAsync(CompetenceName name, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(CompetenceName name, CancellationToken ct = default);
        Task<PagedResult<Competence>> GetPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default);
    }
}
