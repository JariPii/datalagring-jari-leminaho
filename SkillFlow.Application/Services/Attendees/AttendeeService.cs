using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Services.Attendees
{
    public class AttendeeService(IAttendeeRepository repository) : IAttendeeService
    {
        public async Task AddCompetenceToInstructorAsync(Guid instructorId, string competenceName)
        {
            var attendeeId = new AttendeeId(instructorId);

            var attendee = await repository.GetByIdAsync(attendeeId) ??
                throw new AttendeeNotFoundException(attendeeId);

            if (attendee is not Instructor instructor)
            {
                throw new InvalidRoleException(attendeeId, Role.Instructor);
            }

            var cName = CompetenceName.Create(competenceName);
            var cId = CompetenceId.New();
            var competence = new Competence(cId, cName);

            instructor.AddCompetence(competence);

            await repository.UpdateAsync(instructor);
        }

        public async Task<AttendeeDTO> CreateAttendeeAsync(CreateAttendeeDTO dto)
        {
            var email = Email.Create(dto.Email);
            var name = AttendeeName.Create(dto.FirstName, dto.LastName);
            var phone = !string.IsNullOrWhiteSpace(dto.PhoneNumber) ? 
                PhoneNumber.Create(dto.PhoneNumber)
                : null;

            if (await repository.ExistsByEmailAsync(email))
                throw new EmailAlreadyExistsException(email);

            Attendee attendee = dto.Role.ToLower() switch
            {
                "instructor" => new Instructor(AttendeeId.New(), email, name, phone),
                "student" => new Student(AttendeeId.New(), email, name, phone),
                _ => throw new ArgumentException("Invalid role")
            };

            await repository.AddAsync(attendee);

            return new AttendeeDTO
            {
                Id = attendee.Id.Value,
                Email = attendee.Email.Value,
                FirstName = attendee.Name.FirstName,
                LastName = attendee.Name.LastName,
                PhoneNumber = attendee.PhoneNumber?.Value,
                Role = attendee is Instructor ? "Instructor" : "Student"
            }; ;
        }

        public async Task DeleteAttendeeAsync(Guid id)
        {
            var attendeeId = new AttendeeId(id);

            var success = await repository.DeleteAsync(attendeeId);

            if (!success)
                throw new AttendeeNotFoundException(attendeeId);
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAllAttendeesAsync()
        {
            var attendees = await repository.GetAllAsync();
            return MapToDTOList(attendees);
        }

        public async Task<IEnumerable<InstructorDTO>> GetAllInstructorsAsync()
        {
            var instructors = await repository.GetAllInstructorsAsync();

            return instructors.Select(i => new InstructorDTO
            {
                Id = i.Id.Value,
                Email = i.Email.Value,
                FirstName = i.Name.FirstName,
                LastName = i.Name.LastName,
                PhoneNumber = i.PhoneNumber?.Value,
                Role = "Instructor",
                Competences = i.Competences.Select(c => new CompetenceDTO
                {
                    Id = c.Id.Value,
                    Name = c.Name.Value
                }).ToList()
            });
        }

        public async Task<AttendeeDTO> GetAttendeeByEmailAsync(string email)
        {
            var attendeeEmail = Email.Create(email);

            var attendee = await repository.GetByEmailAsync(attendeeEmail) ??
                throw new AttendeeNotFoundException(attendeeEmail);

            return MapToDTOList([attendee]).First();
        }

        public async Task<AttendeeDTO> GetAttendeeByIdAsync(Guid id)
        {
            var attendeeId = new AttendeeId(id);

            var attendee = await repository.GetByIdAsync(attendeeId) ??
                throw new AttendeeNotFoundException(attendeeId);

            return MapToDTOList([attendee]).First();
        }

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByFirstNameAsync(string firstName)
         => await SearchAttendeesByNameAsync(firstName);

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByLastNameAsync(string lastName)
         => await SearchAttendeesByNameAsync(lastName);

        public async Task<IEnumerable<AttendeeDTO>> GetAttendeesByRoleAsync(string role)
        {
            if(string.IsNullOrWhiteSpace(role) || !Enum.TryParse<Role>(role, true, out var parsedRole))
            {
                throw new InvalidRoleException(role ?? "Unknown");
            }

            var attendees = await repository.SearchByRoleAsync(parsedRole);
            return MapToDTOList(attendees);
        }

        public async Task<IEnumerable<InstructorDTO>> GetInstructorsByCompetenceAsync(string competence)
        {
            var instructors = await repository.GetInstructorsByCompetenceAsync(competence);

            return MapToDTOList(instructors).Cast<InstructorDTO>();
        }

        public async Task<IEnumerable<AttendeeDTO>> SearchAttendeesByNameAsync(string searchTerm)
        {
            var attendees = await repository.SearchByNameAsync(searchTerm);
            return MapToDTOList(attendees);

        }

        public async Task UpdateAttendeeAsync(UpdateAttendeeDTO dto)
        {
            var attendeeId = new AttendeeId(dto.Id);

            var attendee = await repository.GetByIdAsync(attendeeId) ??
                throw new AttendeeNotFoundException(attendeeId);

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != attendee.Email.Value)
            {
                var newEmail = Email.Create(dto.Email);

                if(await repository.ExistsByEmailAsync(newEmail))
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

            await repository.UpdateAsync(attendee);
        }

        private IEnumerable<AttendeeDTO> MapToDTOList(IEnumerable<Attendee> attendees)
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
                    Role = "Instructor",
                    Competences = i.Competences.Select(c => new CompetenceDTO
                    {
                        Id = c.Id.Value,
                        Name = c.Name.Value
                    }).ToList(),
                    
                },
                _ => new AttendeeDTO
                {
                    Id = a.Id.Value,
                    Email = a.Email.Value,
                    FirstName = a.Name.FirstName,
                    LastName = a.Name.LastName,
                    PhoneNumber = a.PhoneNumber?.Value,
                    Role = a is Student ? "Student" : "Attendee"
                }
            });
        }
    }
}
