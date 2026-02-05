

using SkillFlow.Application.DTOs.Competences;

namespace SkillFlow.Application.Interfaces
{
    public interface ICompetenceService
    {
        Task<IEnumerable<CompetenceDetailsDTO>> GetAllCompetencesAsync(CancellationToken ct = default);
        Task<CompetenceDetailsDTO> GetCompetenceDetailsAsync(Guid id, CancellationToken ct = default);
        Task<CompetenceDTO> CreateCompetenceAsync(CreateCompetenceDTO dto, CancellationToken ct = default);
        Task<CompetenceDTO> UpdateCompetenceAsync(UpdateCompetenceDTO dto, CancellationToken ct = default);
        Task DeleteCompetenceAsync(Guid id, CancellationToken ct = default);
    }
}
