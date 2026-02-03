using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Interfaces
{
    public interface IAttendeeQueries
    {
        Task<IEnumerable<Attendee>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Instructor>> GetAllInstructorsAsync(CancellationToken ct = default);
        Task<IEnumerable<Student>> GetAllStudentsAsync(CancellationToken ct = default);
        Task<IEnumerable<Instructor>> GetInstructorsByCompetenceAsync(string competenceName, CancellationToken ct = default);
        Task<IEnumerable<Attendee>> SearchByNameAsync(string searchTerm, CancellationToken ct = default);
        Task<IEnumerable<Attendee>> SearchByRoleAsync(Role role, CancellationToken ct = default);
        Task<Competence?> GetCompetenceByNameAsync(CompetenceName name, CancellationToken ct = default);
    }
}
