using SkillFlow.Domain.Entities.Competences;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICompetenceRepository : IBaseRepository<Competence, CompetenceId>
    {
        Task<Competence?> GetByNameAsync(CompetenceName name, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(CompetenceName name, CancellationToken ct = default);
    }
}
