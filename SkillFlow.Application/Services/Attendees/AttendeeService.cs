using Microsoft.EntityFrameworkCore;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure;

namespace SkillFlow.Application.Services.Attendees
{
    public class AttendeeService(IAttendeeRepository repository) : IAttendeeService
    {
        public async Task AddCompetenceToInstructorAsync(Guid instructorId, string competenceName, CancellationToken ct)
        {
            var attendeeId = new AttendeeId(instructorId);

            var attendee = await repository.GetByIdAsync(attendeeId, ct) ??
                throw new AttendeeNotFoundException(attendeeId);

            if (attendee is not Instructor instructor)
            {
                throw new InvalidRoleException(attendeeId, Role.Instructor);
            }

            var cName = CompetenceName.Create(competenceName);

            var competence = await repository.GetCompetenceByNameAsync(cName, ct) ??
                throw new CompetenceNotFoundException(cName);

            instructor.AddCompetence(competence);

            await repository.UpdateAsync(instructor, ct);
        }

        public async Task<AttendeeDTO> CreateAttendeeAsync(CreateAttendeeDTO dto, CancellationToken ct)
        {
            var email = Email.Create(dto.Email);
            var name = AttendeeName.Create(dto.FirstName, dto.LastName);
            var phone = !string.IsNullOrWhiteSpace(dto.PhoneNumber) ? 
                PhoneNumber.Create(dto.PhoneNumber)
                : null;

            if (await repository.ExistsByEmailAsync(email, ct))
                throw new EmailAlreadyExistsException(email);

            Attendee attendee = dto.Role switch
            {
                Role.Instructor => new Instructor(AttendeeId.New(), email, name, phone),
                Role.Student => new Student(AttendeeId.New(), email, name, phone),
                _ => throw new ArgumentException("Invalid role")
            };

            await repository.AddAsync(attendee, ct);

            return MapToDTOList([attendee]).First();
        }

        public async Task DeleteAttendeeAsync(Guid id, CancellationToken ct)
        {
            var attendeeId = new AttendeeId(id);

            var success = await repository.DeleteAsync(attendeeId, ct);

            if (!success)
                throw new AttendeeNotFoundException(attendeeId);
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAllAttendeesAsync(CancellationToken ct)
        {
            var attendees = await repository.GetAllAsync(ct);
            return MapToDTOList(attendees);
        }

        public async Task<IEnumerable<InstructorDTO>> GetAllInstructorsAsync(CancellationToken ct)
        {
            var instructors = await repository.GetAllInstructorsAsync(ct);

            return [.. MapToDTOList(instructors).Cast<InstructorDTO>()];

        }

        public async Task<AttendeeDTO> GetAttendeeByEmailAsync(string email, CancellationToken ct)
        {
            var attendeeEmail = Email.Create(email);

            var attendee = await repository.GetByEmailAsync(attendeeEmail, ct) ??
                throw new AttendeeNotFoundException(attendeeEmail);

            return MapToDTOList([attendee]).First();
        }

        public async Task<AttendeeDTO> GetAttendeeByIdAsync(Guid id, CancellationToken ct)
        {
            var attendeeId = new AttendeeId(id);

            var attendee = await repository.GetByIdAsync(attendeeId, ct) ??
                throw new AttendeeNotFoundException(attendeeId);

            return MapToDTOList([attendee]).First();
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByFirstNameAsync(string firstName, CancellationToken ct)
         => await SearchAttendeesByNameAsync(firstName, ct);

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByLastNameAsync(string lastName, CancellationToken ct)
         => await SearchAttendeesByNameAsync(lastName, ct);

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByRoleAsync(string role, CancellationToken ct)
        {
            if(string.IsNullOrWhiteSpace(role) || !Enum.TryParse<Role>(role, true, out var parsedRole))
            {
                throw new InvalidRoleException(role ?? "Unknown");
            }

            var attendees = await repository.SearchByRoleAsync(parsedRole, ct);
            return MapToDTOList(attendees);
        }

        public async Task<IEnumerable<InstructorDTO>> GetInstructorsByCompetenceAsync(string competence, CancellationToken ct)
        {
            var instructors = await repository.GetInstructorsByCompetenceAsync(competence, ct);

            return MapToDTOList(instructors).Cast<InstructorDTO>();
        }

        public async Task<IEnumerable<AttendeeDTO>> SearchAttendeesByNameAsync(string searchTerm, CancellationToken ct)
        {
            var attendees = await repository.SearchByNameAsync(searchTerm, ct);
            return MapToDTOList(attendees);

        }

        public async Task UpdateAttendeeAsync(UpdateAttendeeDTO dto, CancellationToken ct)
        {
            var attendeeId = new AttendeeId(dto.Id);

            var attendee = await repository.GetByIdAsync(attendeeId, ct) ??
                throw new AttendeeNotFoundException(attendeeId);

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != attendee.Email.Value)
            {
                var newEmail = Email.Create(dto.Email);

                if(await repository.ExistsByEmailAsync(newEmail, ct))
                {
                    throw new EmailAlreadyExistsException(newEmail);
                }

                attendee.UpdateEmail(dto.Email);
            }

            attendee.UpdateFirstName(dto.FirstName ?? attendee.Name.FirstName);
            attendee.UpdateLastName(dto.LastName ?? attendee.Name.LastName);

            if (dto.PhoneNumber != null)
            {
                attendee.UpdatePhoneNumber(PhoneNumber.Create(dto.PhoneNumber));
            }

            await repository.UpdateAsync(attendee, ct);
        }

        private static IEnumerable<AttendeeDTO> MapToDTOList(IEnumerable<Attendee> attendees)
        {
            return attendees.Select(a => a switch
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
                        Name = c.Name.Value
                    })],

                },
                _ => new AttendeeDTO
                {
                    Id = a.Id.Value,
                    Email = a.Email.Value,
                    FirstName = a.Name.FirstName,
                    LastName = a.Name.LastName,
                    PhoneNumber = a.PhoneNumber?.Value,
                    Role = a.Role
                }
            });
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAllStudentsAsync(CancellationToken ct = default)
        {
            var students = await repository.GetAllStudentsAsync(ct);

            return MapToDTOList(students);
        }
    }

}
