using SkillFlow.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(CourseId id);
        Task<Course?> GetByCourseCodeAsync(CourseCode code);
        Task<Course?> GetByCourseNameAsync(CourseName name);

        Task<bool> ExistsByIdAsync(CourseId id);
        Task<bool> ExistsByCourseCodeAsync(CourseCode code);
        Task<bool> ExistsByCourseName(CourseName name);

        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm);

        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task<bool> DeleteAsync(CourseId id);
    }
}
