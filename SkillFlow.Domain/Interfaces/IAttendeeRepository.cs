using SkillFlow.Domain.Entities.Attendees;

namespace SkillFlow.Domain.Interfaces
{
    public interface IAttendeeRepository : IBaseRepository<Attendee, AttendeeId>
    {
        Task<Attendee?> GetByEmailAsync(Email email, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
        Task<bool> ExistsByIdAsync(AttendeeId id, CancellationToken ct = default);
        Task<List<Instructor>> GetInstructorsByIdsAsync(IReadOnlyCollection<AttendeeId> ids, CancellationToken ct);
    }
}
