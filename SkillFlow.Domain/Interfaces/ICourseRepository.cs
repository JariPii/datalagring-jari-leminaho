using SkillFlow.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(CourseId id, CancellationToken ct = default);
        Task<Course?> GetByCourseCodeAsync(CourseCode code, CancellationToken ct = default);
        Task<Course?> GetByCourseNameAsync(CourseName name, CancellationToken ct = default);

        Task<bool> ExistsByIdAsync(CourseId id, CancellationToken ct = default);
        Task<bool> ExistsByCourseCodeAsync(CourseCode code, CancellationToken ct = default);
        Task<bool> ExistsByCourseName(CourseName name, CancellationToken ct = default);

        Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);

        Task AddAsync(Course course, CancellationToken ct = default);
        Task UpdateAsync(Course course, CancellationToken ct = default);
        Task<bool> DeleteAsync(CourseId id, CancellationToken ct = default);
    }
}
