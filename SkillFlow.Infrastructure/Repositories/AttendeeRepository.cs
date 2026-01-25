using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Infrastructure.Repositories
{
    public class AttendeeRepository : IAttendeeRepository
    {
        private readonly SkillFlowDbContext _context;

        public AttendeeRepository(SkillFlowDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Attendee attendee)
        {
            await _context.Attendees.AddAsync(attendee);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(AttendeeId id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee is null) return false;

            try
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> ExistsByEmailAsync(Email email)
        {
            return await _context.Attendees.AnyAsync(e => e.Email == email);
        }

        public async Task<bool> ExistsByIdAsync(AttendeeId id)
        {
            return await _context.Attendees.AnyAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Attendee>> GetAllAsync()
        {
            return await _context.Attendees.ToListAsync();
        }

        public async Task<IEnumerable<Instructor>> GetAllInstructorsAsync()
        {
            return await _context.Instructors.ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Attendee?> GetByEmailAsync(Email email) 
            => await _context.Attendees.FirstOrDefaultAsync(e => e.Email == email);

        public async Task<Attendee?> GetByIdAsync(AttendeeId id) 
            => await _context.Attendees.FindAsync(id);

        public async Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName)
        {
            var pattern = $"%{competenceName}%";
            return await _context.Instructors
                .FromSqlInterpolated($@"
                    SELECT a.* FROM Attendees a 
                    JOIN InstructorCompetences ic ON a.Id = ic.InstructorsId
                    JOIN Competences c ON ic.CompetencesId = c.id
                    WHERE c.Name LIKE {pattern}")
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendee>> SearchByNameAsync(string searchTerm)
        {
            var searchPattern = $"%{searchTerm}%";

            return await _context.Attendees
                .FromSqlInterpolated($@"
                    SELECT * FROM Attendees 
                    WHERE FirstName LIKE {searchPattern}
                    OR LastName LIKE {searchPattern} ")
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendee>> SearchByRoleAsync(Role role)
        {
            var roleName = role.ToString();

            return await _context.Attendees
                .FromSqlInterpolated($@"
                    SELECT * FROM Attendees WHERE Role = {roleName}")
                .ToListAsync();
        }

        public async Task UpdateAsync(Attendee attendee)
        {
            _context.Attendees.Update(attendee);
            await _context.SaveChangesAsync();
        }
    }
}
