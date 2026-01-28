using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Domain.Interfaces
{
    public interface IAttendeeRepository
    {
        Task<Attendee?> GetByIdAsync(AttendeeId id, CancellationToken ct = default);
        Task<Attendee?> GetByEmailAsync(Email email, CancellationToken ct = default);

        Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
        Task<bool> ExistsByIdAsync(AttendeeId id, CancellationToken ct = default);

        Task<IEnumerable<Attendee>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Instructor>> GetAllInstructorsAsync(CancellationToken ct = default);
        Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct = default);
        Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName, CancellationToken ct = default);
        Task<IEnumerable<Attendee>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);
        Task<IEnumerable<Attendee>> SearchByRoleAsync(Role role, CancellationToken ct = default);

        Task AddAsync(Attendee attendee, CancellationToken ct = default);
        Task UpdateAsync(Attendee attendee, CancellationToken ct = default);
        Task<bool> DeleteAsync(AttendeeId id, CancellationToken ct = default);
    }
}
