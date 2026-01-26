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

        public async Task AddAsync(CourseSession session)
        {
            await _context.CourseSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(CourseSessionId id)
        {
            var courseSession = await _context.CourseSessions.FindAsync(id);

            if (courseSession is null) return false;

            try
            {
                _context.CourseSessions.Remove(courseSession);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsByIdAsync(CourseSessionId id)
        {
            return await _context.CourseSessions.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CourseSession>> GetByCourseCodeAsync(CourseCode code)
        {
            return await _context.CourseSessions
                .Include(s => s.Course)
                .Where(s => s.CourseCode == code)
                .ToListAsync();
        }

        public async Task<CourseSession?> GetByIdAsync(CourseSessionId id) => await _context.CourseSessions.FindAsync(id);

        public async Task<CourseSession?> GetByIdWithInstructorsAndEnrollmentsAsync(CourseSessionId id)
        {
            return await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Location)
                .Include(s => s.Instructors)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<CourseSession>> GetByLocationAsync(LocationId locationId)
        {
            return await _context.CourseSessions
                .Include(l => l.Location)
                .Include(l => l.Course)
                .Where(l => l.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseSession>> GetSessionsWithAvailableCapacityAsync()
        {
            return await _context.CourseSessions
                .Include(s => s.Course)
                .Include(s => s.Location)
                .Where(s => s.Enrollments.Count(e => e.Status == EnrollmentStatus.Approved) < s.Capacity)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseSession>> SearchByEndDateAsync(DateTime endDate)
        {
            return await _context.CourseSessions
                .AsNoTracking()
                .Include(s => s.Course)
                .Include(s => s.Location)
                .Where(s => s.EndDate.Date == endDate.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseSession>> SearchByStartDateAsync(DateTime startDate)
        {
            return await _context.CourseSessions
                .AsNoTracking()
                .Include(s => s.Course)
                .Include(s => s.Location)
                .Where(s => s.StartDate.Date == startDate.Date)
                .ToListAsync();
        }

        public async Task UpdateAsync(CourseSession session)
        {
            _context.CourseSessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}
