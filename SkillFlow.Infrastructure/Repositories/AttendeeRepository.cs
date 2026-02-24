using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;
using System.Linq.Expressions;

namespace SkillFlow.Infrastructure.Repositories
{
    public class AttendeeRepository(SkillFlowDbContext context)
        : BaseRespository<Attendee, AttendeeId>(context),
          IAttendeeRepository,
          IAttendeeQueries
    {

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct)
        {
            return await _context.Attendees.AnyAsync(e => e.EmailUnique == email.UniqueValue, ct);
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
                .AsNoTracking()
                .Include(i => i.Competences)
                .AsSplitQuery()
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<List<Instructor>> GetInstructorsByIdsAsync(
            IReadOnlyCollection<AttendeeId> ids, CancellationToken ct)
        {
            if (ids is null)
                throw new InstructorIdsRequiredException();

            if (ids.Count == 0)
                throw new InstructorIdsRequiredException();

            return await _context.Instructors
                .Where(i => ids.Contains(i.Id))
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

        public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(AttendeeId studentId, CancellationToken ct)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .Include(e => e.CourseSession)
                    .ThenInclude(cs => cs.Course)
                .Include(e => e.CourseSession)
                    .ThenInclude(cs => cs.Location)
                .ToListAsync(ct);
        }

        public async Task<PagedResult<Student>> GetStudentsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {

            Expression<Func<Attendee, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();

               filter = s => 
                    EF.Functions.Like(s.Name.FirstName, $"%{term}%") ||
                    EF.Functions.Like(s.Name.LastName, $"%{term}%");
            }

            var result = await GetPagedAsync(page, pageSize, filter, include: query => query.OfType<Student>(), ct: ct);

            var students = result.Items.Cast<Student>();

            return new PagedResult<Student>(students, result.Page, result.PageSize, result.Total);
            
        }

        public async Task<PagedResult<Instructor>> GetInstructorsPagedAsync(int page, int pageSize, string? q, CancellationToken ct = default)
        {

            Expression<Func<Attendee, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();

               filter = i => 
                    EF.Functions.Like(i.Name.FirstName, $"%{term}%") ||
                    EF.Functions.Like(i.Name.LastName, $"%{term}%");
            }

            var result = await GetPagedAsync(page, pageSize, filter, include: query => query.OfType<Instructor>(), ct: ct);

            var instructors = result.Items.Cast<Instructor>();

            return new PagedResult<Instructor>(instructors, result.Page, result.PageSize, result.Total);

        }
    }
}
