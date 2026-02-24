using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICourseRepository : IBaseRepository<Course, CourseId>
    {
        Task<Course?> GetByCourseNameAsync(CourseName name, CancellationToken ct = default);
        Task<Course?> GetByCourseCodeAsync(CourseCode code, CancellationToken ct = default);
        Task<bool> ExistsByCourseName(CourseName name, CancellationToken ct = default);
        Task<bool> ExistsByCourseCodeAsync(CourseCode code, CancellationToken ct = default);
        Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);
        Task<int> GetMaxSuffixAsync(string coursePart, CourseType type, CancellationToken ct = default);
        void Remove(Course course);
        Task<PagedResult<Course>> GetCoursePagedAsync(int page, int pageSize, string? q, CancellationToken ct = default);
        Task<bool> IsCourseInUseAsync(CourseId id, CancellationToken ct = default);

    }
}
