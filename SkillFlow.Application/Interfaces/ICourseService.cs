using SkillFlow.Application.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDTO>> GetAllCoursesAsync();
        Task<CourseDTO> GetCourseByIdAsync(Guid id);
        Task<CourseDTO> GetByCourseCodeAsync(string code);
        Task<CourseDTO> GetCourseByNameAsync(string name);
        Task<IEnumerable<CourseDTO>> SearchCoursesAsync(string searchTerm);
        Task<CourseDTO> CreateCourseAsync(CreateCourseDTO dto);
        Task UpdateCourseAsync(UpdateCourseDTO dto);
        Task DeleteCourseAsync(Guid id);
    }
}
