using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SkillFlowDbContext _context;

        public CourseRepository(SkillFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(CourseId id, CancellationToken ct) => await _context.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);
        
        public async Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm, CancellationToken ct)
        {
            var searchPattern = $"%{searchTerm}%";

            return await _context.Courses
                .FromSqlInterpolated($"SELECT * FROM Courses WHERE CourseName LIKE {searchPattern}")
                .ToListAsync(ct);
        }

        public async Task AddAsync(Course course, CancellationToken ct)
        {
            await _context.Courses.AddAsync(course, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(CourseId id, CancellationToken ct)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (course is null) return false;

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException)
            {
                throw new CourseInUseException(course.CourseCode);
            }           
        }

        public async Task<bool> ExistsByCourseCodeAsync(CourseCode code, CancellationToken ct)
        {
            return await _context.Courses.AnyAsync(c => c.CourseCode == code, ct);
        }

        public async Task<bool> ExistsByIdAsync(CourseId id, CancellationToken ct)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id, ct);
        }

        public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Courses.ToListAsync(ct);
        }

        public async Task<Course?> GetByCourseCodeAsync(CourseCode code, CancellationToken ct)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == code, ct);
        }

        public async Task UpdateAsync(Course course, CancellationToken ct)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsByCourseName(CourseName name, CancellationToken ct)
        {
            return await _context.Courses.AnyAsync(c => c.CourseName == name, ct);
        }

        public async Task<Course?> GetByCourseNameAsync(CourseName name, CancellationToken ct)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseName == name, ct);
        }
    }
}
