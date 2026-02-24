using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using SkillFlow.Infrastructure.Primitives;
using System.Linq.Expressions;

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

        public async Task<bool> ExistsByCourseName(CourseName name, CancellationToken ct)
        {
            return await _context.Courses.AnyAsync(c => c.CourseName == name, ct);
        }

        public async Task<Course?> GetByCourseNameAsync(CourseName name, CancellationToken ct)
        {
            return await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseName == name, ct);
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

        public void Remove(Course course)
        {
            _context.Courses.Remove(course);
        }

        public async Task<PagedResult<Course>> GetCoursePagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {
            Expression<Func<Course, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();

                filter = c => EF.Functions.Like(c.CourseName.Value, $"%{term}%");
            }

            return await GetPagedAsync(page, pageSize, filter, ct: ct);
        }

        public async Task<bool> IsCourseInUseAsync(CourseId id, CancellationToken ct = default)
        {
            return await _context.Set<CourseSession>()
                .AsNoTracking()
                .AnyAsync(cs => cs.CourseId == id, ct);
        }
    }
}
