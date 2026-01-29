using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Infrastructure.Repositories
{
    public class AttendeeRepository : IAttendeeRepository
    {
        private readonly SkillFlowDbContext _context;

        public AttendeeRepository(SkillFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Attendee attendee, CancellationToken ct)
        {
            await _context.Attendees.AddAsync(attendee, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> DeleteAsync(AttendeeId id, CancellationToken ct)
        {
            var attendee = await _context.Attendees.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (attendee is null) return false;

            try
            {
                _context.Attendees.Remove(attendee);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct)
        {
            return await _context.Attendees.AnyAsync(e => e.Email == email, ct);
        }

        public async Task<bool> ExistsByIdAsync(AttendeeId id, CancellationToken ct)
        {
            return await _context.Attendees.AnyAsync(a => a.Id == id, ct);
        }

        //public async Task<IEnumerable<Attendee>> GetAllAsync(CancellationToken ct)
        //{
        //    return await _context.Attendees
        //        .AsNoTracking()
        //        .Include(a => ((Instructor)a).Competences!)
        //        .ToListAsync(ct);
        //}

        public async Task<IEnumerable<Attendee>> GetAllAsync(CancellationToken ct)
        {
            var instructors = await _context.Attendees
                .OfType<Instructor>()
                .Include(i => i.Competences)
                .AsNoTracking()
                .ToListAsync(ct);

            var students = await _context.Attendees
                .OfType<Student>()
                .AsNoTracking()
                .ToListAsync(ct);

            return instructors.Cast<Attendee>().Concat(students);
        }

        public async Task<IEnumerable<Instructor>> GetAllInstructorsAsync(CancellationToken ct)
        {
            return await _context.Instructors.ToListAsync(ct);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct)
        {
            return await _context.Students.ToListAsync(ct);
        }

        public async Task<Attendee?> GetByEmailAsync(Email email, CancellationToken ct) 
            => await _context.Attendees.FirstOrDefaultAsync(e => e.Email == email, ct);

        //public async Task<Attendee?> GetByIdAsync(AttendeeId id, CancellationToken ct) 
        //    => await _context.Attendees.FirstOrDefaultAsync(a => a.Id == id, ct);

        public async Task<Attendee?> GetByIdAsync(AttendeeId id, CancellationToken ct)
        {
            // Vi försöker hämta som Instructor först för att få med kompetenser
            var instructor = await _context.Attendees
                .OfType<Instructor>()
                .Include(i => i.Competences)
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (instructor is not null) return instructor;

            // Om ingen lärare hittades, sök bland studenter
            return await _context.Attendees
                .OfType<Student>()
                .FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName, CancellationToken ct)
        {
            var pattern = $"%{competenceName}%";
            return await _context.Instructors
                .FromSqlInterpolated($@"
                    SELECT a.* FROM Attendees a 
                    JOIN InstructorCompetences ic ON a.Id = ic.InstructorsId
                    JOIN Competences c ON ic.CompetencesId = c.id
                    WHERE c.Name LIKE {pattern}")
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Attendee>> SearchByNameAsync(string searchTerm, CancellationToken ct)
        {
            var searchPattern = $"%{searchTerm}%";

            return await _context.Attendees
                .FromSqlInterpolated($@"
                    SELECT * FROM Attendees 
                    WHERE FirstName LIKE {searchPattern}
                    OR LastName LIKE {searchPattern} ")
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Attendee>> SearchByRoleAsync(Role role, CancellationToken ct)
        {
            var roleName = role.ToString();

            return await _context.Attendees
                .FromSqlInterpolated($@"
                    SELECT * FROM Attendees WHERE Role = {roleName}")
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(Attendee attendee, CancellationToken ct)
        {
            _context.Attendees.Update(attendee);
            await _context.SaveChangesAsync(ct);
        }
    }
}
