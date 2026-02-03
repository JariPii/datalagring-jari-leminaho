using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Domain.Interfaces
{
    public interface IAttendeeRepository
    {
        Task<Attendee?> GetByIdAsync(AttendeeId id, CancellationToken ct = default);
        Task<Attendee?> GetByEmailAsync(Email email, CancellationToken ct = default);

        Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
        Task<bool> ExistsByIdAsync(AttendeeId id, CancellationToken ct = default);

        Task AddAsync(Attendee attendee, CancellationToken ct = default);
        Task UpdateAsync(Attendee attendee, CancellationToken ct = default);
        Task<bool> DeleteAsync(AttendeeId id, CancellationToken ct = default);
    }
}
