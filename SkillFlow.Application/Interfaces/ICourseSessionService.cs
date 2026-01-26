using SkillFlow.Application.DTOs.CourseSessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Interfaces
{
    public interface ICourseSessionService
    {
        Task<IEnumerable<CourseSessionDTO>> GetAllCourseSessionsAsync();
        Task<CourseSessionDTO> GetCourseSessionByIdAsync(Guid id);
        Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByDateAsync(DateTime date);
        Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByLocationAsync(Guid locationId);
        Task<IEnumerable<CourseSessionDTO>> GetAvailableCourseSessionsAsync();
        Task<IEnumerable<CourseSessionDTO>> SearchCourseSessionsAsync(string searchTerm);
        Task<CourseSessionDTO> CreateCourseSessionAsync(CreateCourseSessionDTO dto);
        Task UpdateCourseSessionAsync(UpdateCourseSessionDTO dto);
        Task DeleteCourseSessionAsync(Guid id);
        Task EnrollStudentAsync(Guid sessionId, Guid studentId);
        Task SetEnrollmentStatusAsync(Guid sessionId, Guid studentId, string status);
        Task AddInstructorToCourseSessionAsync(Guid sessionId, Guid instructorId);
    }
}
