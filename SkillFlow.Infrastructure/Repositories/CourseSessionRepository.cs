using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Infrastructure.Repositories;

public class CourseSessionRepository(SkillFlowDbContext context) : BaseRespository<CourseSession, CourseSessionId>(context), ICourseSessionRepository
{
    public async Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code, CancellationToken ct)
    {
        return await _context.CourseSessions
            .Include(s => s.Course)
            .Where(s => s.CourseCode == code)
            .ToListAsync(ct);
    }

    public override async Task<CourseSession?> GetByIdAsync(CourseSessionId id, CancellationToken ct)
        => await _context.CourseSessions.FindAsync([id], ct);

    public async Task<CourseSession?> GetByIdWithInstructorsAndEnrollmentsAsync(CourseSessionId id, CancellationToken ct)
    {
        return await _context.CourseSessions
            .AsSplitQuery()
            .Include(s => s.Course)
            .Include(s => s.Location)
            .Include(s => s.Instructors)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IEnumerable<CourseSession>> GetByLocationAsync(LocationId locationId, CancellationToken ct)
    {
        return await _context.CourseSessions
            .Include(l => l.Location)
            .Include(l => l.Course)
            .Where(l => l.LocationId == locationId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CourseSession>> GetSessionInDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
    {
        return await _context.CourseSessions
            .AsNoTracking()
            .Where(s => s.StartDate >= startDate && s.StartDate <= endDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CourseSession>> GetSessionsWithAvailableCapacityAsync(CancellationToken ct)
    {
        return await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Location)
            .Where(s => s.Enrollments.Count(e => e.Status == EnrollmentStatus.Approved) < s.Capacity)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CourseSession>> SearchByEndDateAsync(DateTime endDate, CancellationToken ct)
    {
        return await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Location)
            .Where(s => s.EndDate.Date == endDate.Date)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CourseSession>> SearchByStartDateAsync(DateTime startDate, CancellationToken ct)
    {
        return await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Location)
            .Where(s => s.StartDate.Date == startDate.Date)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CourseSession>> SearchAsync(string searchTerm, CancellationToken ct)
    {
        var searchPattern = $"%{searchTerm}%";
        return await _context.CourseSessions
            .FromSqlInterpolated($@"
                    SELECT s.*
                    FROM CourseSessions s
                    JOIN Courses c ON s.CourseId = c.Id
                    JOIN Locations l ON s.LocationId = l.Id
                    WHERE c.CourseName LIKE {searchPattern}
                        OR l.LocationName LIKE {searchPattern}
                        OR s.CourseCode LIKE {searchPattern}
                    ")
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Location)
            .Include(i => i.Instructors)
            .ToListAsync(ct);
    }

    public override async Task<IEnumerable<CourseSession>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
            .Include(s => s.Location)
            .Include(s => s.Enrollments)
            .Include(s => s.Instructors)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsBySessionIdAsync(CourseSessionId sessionId, CancellationToken ct = default)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Where(e => e.CourseSessionId == sessionId)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<CourseSession>> GetCourseSessionsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
    {
        Expression<Func<CourseSession, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            filter = s => EF.Functions.Like(s.Course.CourseName.Value, $"%{term}%");
        }

        return await GetPagedAsync(
            page,
            pageSize,
            filter,
            include: i => i.Include(s => s.Course).Include(s => s.Location).Include(s => s.Instructors).Include(s => s.Enrollments),
            ct);
    }
}