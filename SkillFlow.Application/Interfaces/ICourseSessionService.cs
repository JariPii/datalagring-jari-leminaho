using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Application.Interfaces
{
    public interface ICourseSessionService
    {
        Task<IEnumerable<CourseSessionDTO>> GetAllCourseSessionsAsync(CancellationToken ct = default);
        Task<CourseSessionDTO> GetCourseSessionByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByDateAsync(DateTime date, CancellationToken ct = default);
        Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByLocationAsync(Guid locationId, CancellationToken ct = default);
        Task<IEnumerable<CourseSessionDTO>> GetAvailableCourseSessionsAsync(CancellationToken ct = default);
        Task<IEnumerable<CourseSessionDTO>> SearchCourseSessionsAsync(string searchTerm, CancellationToken ct = default);
        Task<CourseSessionDTO> CreateCourseSessionAsync(CreateCourseSessionDTO dto, CancellationToken ct = default);
        Task UpdateCourseSessionAsync(UpdateCourseSessionDTO dto, CancellationToken ct = default);
        Task DeleteCourseSessionAsync(Guid id, CancellationToken ct = default);
        Task EnrollStudentAsync(Guid sessionId, Guid studentId, CancellationToken ct = default);
        Task SetEnrollmentStatusAsync(Guid sessionId, Guid studentId, EnrollmentStatus status, CancellationToken ct = default);
        Task AddInstructorToCourseSessionAsync(Guid sessionId, Guid instructorId, CancellationToken ct = default);
        Task<IReadOnlyList<EnrollmentDTO>> GetEnrollmentsBySessionIdAsync(Guid sessionId, CancellationToken ct = default);
    }
}
