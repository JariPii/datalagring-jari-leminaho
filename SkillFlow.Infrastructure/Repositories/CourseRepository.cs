using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SkillFlowDbContext _context;

        public CourseRepository(SkillFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(CourseId id) => await _context.Courses.FindAsync(id);
        
        public async Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm)
        {
            var searchPattern = $"%{searchTerm}%";

            return await _context.Courses
                .FromSqlInterpolated($"SELECT * FROM Courses WHERE CourseName LIKE {searchPattern}")
                .ToListAsync();
        }

        public Task AddAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(CourseId id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course is null) return false;

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return false;
            }           
        }

        public Task<bool> ExistsByCourseCode(CourseCode code)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByIdAsync(CourseId id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Course>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Course?> GetByCourseCodeAsync(CourseCode code)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
