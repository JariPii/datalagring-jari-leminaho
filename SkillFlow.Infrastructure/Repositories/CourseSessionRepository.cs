using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Infrastructure.Repositories;

public class CourseSessionRepository(SkillFlowDbContext context) : BaseRespository<CourseSession, CourseSessionId>(context), ICourseSessionRepository
{

    public override async Task<bool> DeleteAsync(CourseSessionId id, CancellationToken ct)
    {
        var courseSession = await _context.CourseSessions.FirstOrDefaultAsync(s => s.Id == id, ct);

        if (courseSession is null) return false;

        try
        {
            _context.CourseSessions.Remove(courseSession);
            await _context.SaveChangesAsync(ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code, CancellationToken ct)
    {
        return await _context.CourseSessions
            .Include(s => s.Course)
            .Where(s => s.CourseCode == code)
            .ToListAsync(ct);
    }

    public override async Task<CourseSession?> GetByIdAsync(CourseSessionId id, CancellationToken ct)
        => await _context.CourseSessions.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<CourseSession?> GetByIdWithInstructorsAndEnrollmentsAsync(CourseSessionId id, CancellationToken ct)
    {
        return await _context.CourseSessions
            .Include(s => s.Course)
            .Include(s => s.Location)
            .Include(s => s.Instructors)
            .Include(s => s.Enrollments)
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
                        OR s.CourseCode_Value LIKE {searchPattern}
                    ")
            .Include(s => s.Course)
            .Include(s => s.Location)
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

    public async Task<int> CountSessionsForCourseAndYear(string cityPart, string coursePart, int year, CancellationToken ct = default)
    {
        return await _context.CourseSessions
            .AsNoTracking()
            .CountAsync(s => s.CourseCode.CityPart == cityPart &&
            s.CourseCode.CoursePart == coursePart &&
            s.CourseCode.CourseYear == year, ct);
    }
}