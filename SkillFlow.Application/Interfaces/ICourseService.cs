using SkillFlow.Application.DTOs.Courses;

namespace SkillFlow.Application.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDTO>> GetAllCoursesAsync(CancellationToken ct = default);
        Task<CourseDTO> GetCourseByIdAsync(Guid id, CancellationToken ct = default);
        Task<CourseDTO> GetByCourseCodeAsync(string code, CancellationToken ct = default);
        Task<CourseDTO> GetCourseByNameAsync(string name, CancellationToken ct = default);
        Task<IEnumerable<CourseDTO>> SearchCoursesAsync(string searchTerm, CancellationToken ct = default);
        Task<CourseDTO> CreateCourseAsync(CreateCourseDTO dto, CancellationToken ct = default);
        Task UpdateCourseAsync(UpdateCourseDTO dto, CancellationToken ct = default);
        Task DeleteCourseAsync(Guid id, CancellationToken ct = default);
    }
}
