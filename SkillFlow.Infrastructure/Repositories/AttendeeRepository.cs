using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Infrastructure.Repositories
{
    public class AttendeeRepository(SkillFlowDbContext context)
        : BaseRespository<Attendee, AttendeeId>(context),
          IAttendeeRepository,
          IAttendeeQueries
    {

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
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct)
        {
            return await _context.Students.AsNoTracking().OrderByDescending(s => s.CreatedAt).ToListAsync(ct);
        }

        public async Task<Attendee?> GetByEmailAsync(Email email, CancellationToken ct) 
            => await _context.Attendees.FirstOrDefaultAsync(e => e.Email == email, ct);

        public override async Task<Attendee?> GetByIdAsync(AttendeeId id, CancellationToken ct)
        {
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (attendee is Instructor instructor)
            {
                await _context.Entry(instructor)
                    .Collection(i => i.Competences)
                    .LoadAsync(ct);
            }

            return attendee;
        }

        public async Task<Competence?> GetCompetenceByNameAsync(CompetenceName name, CancellationToken ct = default)
        {
            return await _context.Set<Competence>()
                .FirstOrDefaultAsync(c => c.Name == name, ct);
        }

        public async Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(competenceName))
                return [];

            var pattern = $"%{competenceName.Trim()}%";

            return await _context.Instructors
                .FromSqlInterpolated($@"
                    SELECT a.*, a.FirstName AS Name_FirstName, a.LastName AS Name_LastName
                    FROM Attendees AS a
                    INNER JOIN InstructorCompetences AS ic ON a.Id = ic.InstructorsId
                    INNER JOIN Competences AS c ON ic.CompetencesId = c.Id
                    WHERE c.Name LIKE {pattern}
                    AND a.Role = 'Instructor'")
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Attendee>> SearchByNameAsync(string searchTerm, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return [];

            var searchPattern = $"%{searchTerm.Trim()}%";

            return await _context.Attendees
                .FromSqlInterpolated($@"
                    SELECT *, FirstName AS Name_FirstName, LastName AS Name_LastName
                    FROM Attendees 
                    WHERE FirstName LIKE {searchPattern}
                    OR LastName LIKE {searchPattern} ")
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
