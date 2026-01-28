using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.CourseSessions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Repositories
{
    internal class CourseSessionRepository : ICourseSessionRepository
    {
        private readonly SkillFlowDbContext _context;

        public CourseSessionRepository(SkillFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CourseSession session, CancellationToken ct)
        {
            await _context.CourseSessions.AddAsync(session, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(CourseSessionId id, CancellationToken ct)
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

        public async Task<bool> ExistsByIdAsync(CourseSessionId id, CancellationToken ct)
        {
            return await _context.CourseSessions.AnyAsync(c => c.Id == id, ct);
        }

        public async Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code, CancellationToken ct)
        {
            return await _context.CourseSessions
                .Include(s => s.Course)
                .Where(s => s.CourseCode == code)
                .ToListAsync(ct);
        }

        public async Task<CourseSession?> GetByIdAsync(CourseSessionId id, CancellationToken ct)
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

        public async Task UpdateAsync(CourseSession session, CancellationToken ct)
        {
            _context.CourseSessions.Update(session);
            await _context.SaveChangesAsync(ct);
        }
    }
}
