using SkillFlow.Domain.Courses;
using SkillFlow.Domain.CourseSessions;
using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICourseSessionRepository
    {
        Task<CourseSession?> GetByIdAsync(CourseSessionId id, CancellationToken ct = default);
        Task<CourseSession?> GetByIdWithInstructorsAndEnrollmentsAsync(CourseSessionId id, CancellationToken ct = default);

        Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> GetByLocationAsync(LocationId locationId, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> SearchByStartDateAsync(DateTime startDate, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> SearchByEndDateAsync(DateTime endDate, CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> GetSessionInDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);

        Task<IEnumerable<CourseSession>> GetSessionsWithAvailableCapacityAsync(CancellationToken ct = default);
        Task<IEnumerable<CourseSession>> SearchAsync(string searchTerm, CancellationToken ct = default);

        Task<bool> ExistsByIdAsync(CourseSessionId id, CancellationToken ct = default);

        Task AddAsync(CourseSession session, CancellationToken ct = default);
        Task UpdateAsync(CourseSession session, CancellationToken ct = default);
        Task<bool> DeleteAsync(CourseSessionId id, CancellationToken ct = default);
    }
}
