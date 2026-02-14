using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services.Competences
{
    public class CompetenceService(ICompetenceRepository repository, IUnitOfWork unitOfWork) : ICompetenceService
    {
        public async Task<CompetenceDTO> CreateCompetenceAsync(CreateCompetenceDTO dto, CancellationToken ct = default)
        {
            var name = CompetenceName.Create(dto.Name);

            if (await repository.ExistsByNameAsync(name, ct))
                throw new CompetenceNameAllreadyExistsException(name);

            var competence = Competence.Create(name);

            await repository.AddAsync(competence, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(competence);
        }

        public async Task DeleteCompetenceAsync(Guid id, CancellationToken ct = default)
        {
            var competenceId = new CompetenceId(id);

            var success = await repository.DeleteAsync(competenceId, ct);

            if (!success)
                throw new CompetenceNotFoundException(competenceId);

            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<CompetenceDetailsDTO>> GetAllCompetencesAsync(CancellationToken ct = default)
        {
            var competences = await repository.GetAllAsync(ct);
            return MapToDTODetails(competences);
        }

        public async Task<CompetenceDetailsDTO> GetCompetenceDetailsAsync(Guid id, CancellationToken ct = default)
        {
            var competenceId = new CompetenceId(id);

            var competence = await repository.GetByIdAsync(competenceId, ct) ??
                throw new CompetenceNotFoundException(competenceId);

            return MapToDTODetails([competence]).First();
        }

        public async Task<CompetenceDTO> UpdateCompetenceAsync(Guid id, UpdateCompetenceDTO dto, CancellationToken ct = default)
        {
            var competenceId = new CompetenceId(id);
            var competence = await repository.GetByIdAsync(competenceId, ct) ??
                throw new CompetenceNotFoundException(competenceId);

            var newName = CompetenceName.Create(dto.Name);

            if (competence.Name != newName && await repository.ExistsByNameAsync(newName, ct))
                throw new CompetenceNameAllreadyExistsException(newName);

            competence.UpdateCompetenceName(newName);

            await repository.UpdateAsync(competence, dto.RowVersion, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(competence);
        }

        private static CompetenceDTO MapToDTO(Competence competence)
        {
            return new CompetenceDTO()
            {
                Id = competence.Id.Value,
                Name = competence.Name.Value,
                RowVersion = competence.RowVersion
            };
        }

        private static IEnumerable<CompetenceDetailsDTO> MapToDTODetails(IEnumerable<Competence> competences)
        {
            return competences.Select(c => new CompetenceDetailsDTO
            {
                Id = c.Id.Value,
                Name = c.Name.Value,
                RowVersion = c.RowVersion,

                Instructors = [.. c.Instructors.Select(i => MapAttendeeToDTO(i))]
            }
            );
        }

        private static AttendeeDTO MapAttendeeToDTO(Attendee a)
        {
            return new AttendeeDTO
            {
                Id = a.Id.Value,
                FirstName = a.Name.FirstName,
                LastName = a.Name.LastName,
                Email = a.Email.Value,
                Role = a.Role,
                PhoneNumber = a.PhoneNumber?.Value,
                RowVersion = a.RowVersion
            };
        }
    }
}
