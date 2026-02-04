using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services.Competences
{
    public class CompetenceService(ICompetenceRepository repository) : ICompetenceService
    {
        public async Task<CompetenceDTO> CreateCompetenceAsync(CreateCompetenceDTO dto, CancellationToken ct = default)
        {
            var name = CompetenceName.Create(dto.Name);

            if (await repository.ExistsByNameAsync(name, ct))
                throw new CompetenceNameAllreadyExistsException(name);

            var competence = Competence.Create(name);

            await repository.AddAsync(competence, ct);

            return MapToDTO(competence);
        }

        public Task DeleteCompetenceAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CompetenceDetailsDTO>> GetAllCompetencesAsync(CancellationToken ct = default)
        {
            var competences = await repository.GetAllAsync(ct);
            return MapToDTODetails(competences);
        }

        public Task<CompetenceDetailsDTO> GetCompetenceDetailsAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCompetenceAsync(UpdateCompetenceDTO dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        private static CompetenceDTO MapToDTO(Competence competence)
        {
            return new CompetenceDTO()
            {
                Id = competence.Id.Value,
                Name = competence.Name.Value
            };
        }

        private static IEnumerable<CompetenceDetailsDTO> MapToDTODetails(IEnumerable<Competence> competences)
        {
            return competences.Select(c => new CompetenceDetailsDTO
            {
                Id = c.Id.Value,
                Name = c.Name.Value,

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
                PhoneNumber = a.PhoneNumber?.Value
            };
        }
    }
}
