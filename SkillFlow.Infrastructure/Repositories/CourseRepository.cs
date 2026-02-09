using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure.Primitives;

namespace SkillFlow.Infrastructure.Repositories
{
    public class CourseRepository(SkillFlowDbContext context) : BaseRespository<Course, CourseId>(context), ICourseRepository
    {
        public async Task<IEnumerable<Course>> SearchByNameAsync(string searchTerm, CancellationToken ct)
        {
            var searchPattern = $"%{searchTerm}%";

            return await _context.Courses
                .FromSqlInterpolated($"SELECT * FROM Courses WHERE CourseName LIKE {searchPattern}")
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public override async Task<bool> DeleteAsync(CourseId id, CancellationToken ct)
        {

            var course = await GetByIdAsync(id, ct);

            if (course is null) return false;

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException)
            {
                throw new CourseInUseException(course.CourseName);
            }
        }

        public async Task<bool> ExistsByCourseName(CourseName name, CancellationToken ct)
        {
            return await _context.Courses.AnyAsync(c => c.CourseName == name, ct);
        }

        public async Task<Course?> GetByCourseNameAsync(CourseName name, CancellationToken ct)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseName == name, ct);
        }

        public async Task<bool> ExistsByCourseCodeAsync(CourseCode code, CancellationToken ct = default)
        {
            return await _context.Courses.AnyAsync(c => c.CourseCode == code, ct);
        }

        public async Task<int> GetMaxSuffixAsync(string coursePart, CourseType type, CancellationToken ct = default)
        {
            var prefix = $"{coursePart}{type}-";
            var pattern = $"{prefix}%";

            var result = await _context.Set<IntResult>()
                .FromSqlInterpolated($@"
                SELECT COALESCE(MAX(CAST(RIGHT(CourseCode, 3) AS int)), 0) AS Value
                FROM Courses
                WHERE CourseCode LIKE {pattern}
                ")
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            return result?.Value ?? 0;
        }

        public async Task<Course?> GetByCourseCodeAsync(CourseCode code, CancellationToken ct = default)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == code, ct);
        }
    }
}
