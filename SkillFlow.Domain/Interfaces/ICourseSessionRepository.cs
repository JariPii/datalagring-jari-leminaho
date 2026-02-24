using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICourseSessionRepository : IBaseRepository<CourseSession, CourseSessionId>
    {
        Task<CourseSession?> GetByIdWithInstructorsAndEnrollmentsAsync(CourseSessionId id, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> GetByLocationAsync(LocationId locationId, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> SearchByStartDateAsync(DateTime startDate, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> SearchByEndDateAsync(DateTime endDate, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> GetSessionInDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> GetSessionsWithAvailableCapacityAsync(CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> SearchAsync(string searchTerm, CancellationToken ct = default);
        Task<bool> ExistsByIdAsync(CourseSessionId id, CancellationToken ct = default);
        Task<PagedResult<CourseSession>> GetCourseSessionsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default);
    }
}
