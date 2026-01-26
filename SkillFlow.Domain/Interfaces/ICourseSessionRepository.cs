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
        Task<CourseSession?> GetByIdAsync(CourseSessionId id);
        Task<CourseSession?> GetByIdWithInstructorsAndEnrollmentsAsync(CourseSessionId id);

        Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code);
        Task<IEnumerable<CourseSession>> GetByLocationAsync(LocationId locationId);
        Task<IEnumerable<CourseSession>> SearchByStartDateAsync(DateTime startDate);
        Task<IEnumerable<CourseSession>> SearchByEndDateAsync(DateTime endDate);

        Task<IEnumerable<CourseSession>> GetSessionsWithAvailableCapacityAsync();

        Task<bool> ExistsByIdAsync(CourseSessionId id);

        Task AddAsync(CourseSession session);
        Task UpdateAsync(CourseSession session);
        Task<bool> DeleteAsync(CourseSessionId id);
    }
}
