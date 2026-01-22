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

        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> SearchByNameAsync(CourseName searhTerm);

        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(CourseId id);

    }
}
