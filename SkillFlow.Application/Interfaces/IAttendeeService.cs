using SkillFlow.Application.DTOs.Attendees;

namespace SkillFlow.Application.Interfaces
{
    public interface IAttendeeService
    {
        Task<IEnumerable<AttendeeDTO>> GetAllAttendeesAsync(CancellationToken ct = default);
        Task<AttendeeDTO> GetAttendeeByIdAsync(Guid id, CancellationToken ct = default);
        Task<AttendeeDTO> GetAttendeeByEmailAsync(string email, CancellationToken ct = default);
        Task<IEnumerable<AttendeeDTO>> GetAttendeesByFirstNameAsync(string firstName, CancellationToken ct = default);
        Task<IEnumerable<AttendeeDTO>> GetAttendeesByLastNameAsync(string lastName, CancellationToken ct = default);
        Task<IEnumerable<AttendeeDTO>> GetAttendeesByRoleAsync(string role, CancellationToken ct = default);
        Task<IEnumerable<InstructorDTO>> GetAllInstructorsAsync(CancellationToken ct = default);
        Task<IEnumerable<AttendeeDTO>> GetAllStudentsAsync(CancellationToken ct = default);
        Task<IEnumerable<InstructorDTO>> GetInstructorsByCompetenceAsync(string competence, CancellationToken ct = default);
        Task<IEnumerable<AttendeeDTO>> SearchAttendeesByNameAsync(string searchTerm, CancellationToken ct = default);
        Task<AttendeeDTO> CreateAttendeeAsync(CreateAttendeeDTO dto, CancellationToken ct = default);
        Task<AttendeeDTO> UpdateAttendeeAsync(UpdateAttendeeDTO dto, CancellationToken ct = default);
        Task DeleteAttendeeAsync(Guid id, CancellationToken ct = default);
        Task AddCompetenceToInstructorAsync(Guid instructorId, string competenceName, byte[] rowVersion, CancellationToken ct = default);
    }
}
