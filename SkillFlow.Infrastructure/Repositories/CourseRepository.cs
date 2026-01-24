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

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
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

        public async Task<bool> ExistsByCourseCodeAsync(CourseCode code)
        {
            return await _context.Courses.AnyAsync(c => c.CourseCode == code);
        }

        public async Task<bool> ExistsByIdAsync(CourseId id)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetByCourseCodeAsync(CourseCode code)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == code);
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }
    }
}
