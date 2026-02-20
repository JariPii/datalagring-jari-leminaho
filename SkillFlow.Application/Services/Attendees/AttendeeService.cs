using Microsoft.EntityFrameworkCore;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services.Attendees
{
    public class AttendeeService(IAttendeeRepository repository, IUnitOfWork unitOfWork, IAttendeeQueries queries, ICompetenceRepository competenceRepository) : IAttendeeService
    {
        public async Task AddCompetenceToInstructorAsync(Guid instructorId, string competenceName, byte[] rowVersion, CancellationToken ct)
        {
            await using var tx = await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var attendeeId = new AttendeeId(instructorId);

                var attendee = await repository.GetByIdAsync(attendeeId, ct) ??
                    throw new AttendeeNotFoundException(attendeeId);

                if (attendee is not Instructor instructor)
                {
                    throw new InvalidRoleException(attendeeId, Role.Instructor);
                }

                var cName = CompetenceName.Create(competenceName);

                var competence = await competenceRepository.GetByNameAsync(cName, ct) ??
                    throw new CompetenceNotFoundException(cName);

                instructor.AddCompetence(competence);

                await repository.UpdateAsync(instructor, rowVersion, ct);

                await unitOfWork.SaveChangesAsync(ct);

                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<AttendeeDTO> CreateAttendeeAsync(CreateAttendeeDTO dto, CancellationToken ct)
        {
            var email = Email.Create(dto.Email);
            var name = AttendeeName.Create(dto.FirstName, dto.LastName);
            var phone = PhoneNumber.Create(dto.PhoneNumber);

            if (await repository.ExistsByEmailAsync(email, ct))
                throw new EmailAlreadyExistsException(email);

            Attendee attendee = dto.Role switch
            {
                Role.Student => Attendee.CreateStudent(email, name, phone),
                Role.Instructor => Attendee.CreateInstructor(email, name, phone),
                _ => throw new InvalidRoleException(dto.Role)
            };

            await repository.AddAsync(attendee, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(attendee);
        }

        public async Task DeleteAttendeeAsync(Guid id, CancellationToken ct)
        {
            var attendeeId = new AttendeeId(id);

            var success = await repository.DeleteAsync(attendeeId, ct);

            if (!success)
                throw new AttendeeNotFoundException(attendeeId);

            await unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAllAttendeesAsync(CancellationToken ct)
        {
            var attendees = await queries.GetAllAsync(ct);

            return MapToDTOList(attendees);
        }

        public async Task<IEnumerable<InstructorDTO>> GetAllInstructorsAsync(CancellationToken ct)
        {
            var instructors = await queries.GetAllInstructorsAsync(ct);

            return [.. MapToDTOList(instructors).Cast<InstructorDTO>()];

        }

        public async Task<AttendeeDTO> GetAttendeeByEmailAsync(string email, CancellationToken ct)
        {
            var attendeeEmail = Email.Create(email);

            var attendee = await repository.GetByEmailAsync(attendeeEmail, ct) ??
                throw new AttendeeNotFoundException(attendeeEmail);

            return MapToDTO(attendee);
        }

        public async Task<AttendeeDTO> GetAttendeeByIdAsync(Guid id, CancellationToken ct)
        {
            var attendeeId = new AttendeeId(id);

            var attendee = await repository.GetByIdAsync(attendeeId, ct) ??
                throw new AttendeeNotFoundException(attendeeId);

            return MapToDTO(attendee);
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByFirstNameAsync(string firstName, CancellationToken ct)
         => await SearchAttendeesByNameAsync(firstName, ct);

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByLastNameAsync(string lastName, CancellationToken ct)
         => await SearchAttendeesByNameAsync(lastName, ct);

        public async Task<IEnumerable<InstructorDTO>> GetInstructorsByCompetenceAsync(string competence, CancellationToken ct)
        {
            var instructors = await queries.GetInstructorsByCompetenceAsync(competence, ct);

            return MapToDTOList(instructors).Cast<InstructorDTO>();
        }

        public async Task<IEnumerable<AttendeeDTO>> SearchAttendeesByNameAsync(string searchTerm, CancellationToken ct)
        {
            var attendees = await queries.SearchByNameAsync(searchTerm, ct);
            return MapToDTOList(attendees);

        }

        public async Task<AttendeeDTO> UpdateAttendeeAsync(Guid id, UpdateAttendeeDTO dto, CancellationToken ct)
        {
            var attendeeId = new AttendeeId(id);

            var attendee = await repository.GetByIdAsync(attendeeId, ct) ??
                throw new AttendeeNotFoundException(attendeeId);

            if (dto.Email is not null && dto.Email != attendee.Email.Value)
            {
                var newEmail = Email.Create(dto.Email);

                if(await repository.ExistsByEmailAsync(newEmail, ct))
                {
                    throw new EmailAlreadyExistsException(newEmail);
                }

                attendee.UpdateEmail(newEmail);
            }

            attendee.UpdateFirstName(dto.FirstName ?? attendee.Name.FirstName);
            attendee.UpdateLastName(dto.LastName ?? attendee.Name.LastName);

            if (dto.PhoneNumber is not null && dto.PhoneNumber != attendee.PhoneNumber?.Value)
            {
                attendee.UpdatePhoneNumber(PhoneNumber.Create(dto.PhoneNumber));
            }

            await repository.UpdateAsync(attendee, dto.RowVersion, ct);

            await unitOfWork.SaveChangesAsync(ct);

            return MapToDTO(attendee);
        }

        public static AttendeeDTO MapToDTO(Attendee a)
        {
            return a switch
            {
                Instructor i => new InstructorDTO
                {
                    Id = i.Id.Value,
                    Email = i.Email.Value,
                    FirstName = i.Name.FirstName,
                    LastName = i.Name.LastName,
                    PhoneNumber = i.PhoneNumber?.Value,
                    Role = i.Role,
                    Competences = [.. i.Competences.Select(c => new CompetenceDTO
                    {
                        Id = c.Id.Value,
                        Name = c.Name.Value,
                        RowVersion = c.RowVersion
                    })],
                    RowVersion = i.RowVersion,
                    CreatedAt = i.CreatedAt

                },
                _ => new StudentDTO
                {
                    Id = a.Id.Value,
                    Email = a.Email.Value,
                    FirstName = a.Name.FirstName,
                    LastName = a.Name.LastName,
                    PhoneNumber = a.PhoneNumber?.Value,
                    Role = a.Role,
                    RowVersion = a.RowVersion,
                    CreatedAt = a.CreatedAt
                }
            };
        }

        public static IEnumerable<AttendeeDTO> MapToDTOList(IEnumerable<Attendee> attendees)
        {
            return attendees.Select(MapToDTO);
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAllStudentsAsync(CancellationToken ct = default)
        {
            var students = await queries.GetAllStudentsAsync(ct);

            return MapToDTOList(students);
        }
    }

}
