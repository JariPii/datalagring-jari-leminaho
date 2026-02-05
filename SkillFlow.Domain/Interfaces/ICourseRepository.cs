using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICourseRepository : IBaseRepository<Course, CourseId>
    {
        Task<Course?> GetByCourseNameAsync(CourseName name, CancellationToken ct = default);

        Task<bool> ExistsByCourseName(CourseName name, CancellationToken ct = default);

        Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);

    }
}
