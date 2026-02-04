using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Infrastructure.Repositories
{
    public class AttendeeRepository(SkillFlowDbContext context)
        : BaseRespository<Attendee, AttendeeId>(context),
          IAttendeeRepository,
          IAttendeeQueries
    {

        public override async Task<bool> DeleteAsync(AttendeeId id, CancellationToken ct)
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

        public override async Task<IEnumerable<Attendee>> GetAllAsync(CancellationToken ct)
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
            return await _context.Instructors
                .Include(i => i.Competences)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct)
        {
            return await _context.Students.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Attendee?> GetByEmailAsync(Email email, CancellationToken ct) 
            => await _context.Attendees.FirstOrDefaultAsync(e => e.Email == email, ct);

        public override async Task<Attendee?> GetByIdAsync(AttendeeId id, CancellationToken ct)
        {
            var instructor = await _context.Attendees
                .OfType<Instructor>()
                .Include(i => i.Competences)
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (instructor is not null) return instructor;

            return await _context.Attendees
                .OfType<Student>()
                .FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<Competence?> GetCompetenceByNameAsync(CompetenceName name, CancellationToken ct = default)
        {
            return await _context.Set<Competence>()
                .FirstOrDefaultAsync(c => c.Name == name, ct);
        }

        public async Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName, CancellationToken ct)
        {
            var pattern = $"%{competenceName}%";
            return await _context.Instructors
                .FromSqlInterpolated($@"
                    SELECT a.*, FirstName AS Name_FirstName, LastName AS Name_LastName FROM Attendees a 
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
                    SELECT *, FirstName AS Name_FirstName, LastName AS Name_LastName
                    FROM Attendees 
                    WHERE FirstName LIKE {searchPattern}
                    OR LastName LIKE {searchPattern} ")
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Attendee>> SearchByRoleAsync(Role role, CancellationToken ct)
        {
            var roleName = role.ToString();

            return await _context.Attendees
                .FromSqlInterpolated($@"
                    SELECT *, FirstName AS Name_FirstName, LastName AS Name_LastName 
                    FROM Attendees WHERE Role = {roleName}")
                .ToListAsync(ct);
        }
    }
}
