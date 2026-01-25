using SkillFlow.Domain.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface IAttendeeRepository
    {
        Task<Attendee?> GetByIdAsync(AttendeeId id);
        Task<Attendee?> GetByEmailAsync(Email email);

        Task<bool> ExistsByEmailAsync(Email email);
        Task<bool> ExistsByIdAsync(AttendeeId id);

        Task<IEnumerable<Attendee>> GetAllAsync();
        Task<IEnumerable<Instructor>> GetAllInstructorsAsync();
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName);
        Task<IEnumerable<Attendee>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Attendee>> SearchByRoleAsync(Role role);

        Task AddAsync(Attendee attendee);
        Task UpdateAsync(Attendee attendee);
        Task<bool> DeleteAsync(AttendeeId id);
    }
}
