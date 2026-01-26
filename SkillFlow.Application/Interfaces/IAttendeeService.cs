using SkillFlow.Application.DTOs.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Interfaces
{
    public interface IAttendeeService
    {
        Task<IEnumerable<AttendeeDTO>> GetAllAttendeesAsync();
        Task<AttendeeDTO> GetAttendeeByIdAsync(Guid id);
        Task<AttendeeDTO> GetAttendeeByEmailAsync(string email);
        Task<IEnumerable<AttendeeDTO>> GetAttendeesByFirstNameAsync(string firstName);
        Task<IEnumerable<AttendeeDTO>> GetAttendeesByLastNameAsync(string lastName);
        Task<IEnumerable<AttendeeDTO>> GetAttendeesByRoleAsync(string role);
        Task<IEnumerable<InstructorDTO>> GetAllInstructorsAsync();
        Task<IEnumerable<InstructorDTO>> GetInstructorsByCompetenceAsync(string competence);
        Task<IEnumerable<AttendeeDTO>> SearchAttendeesByNameAsync(string searchTerm);
        Task<AttendeeDTO> CreateAttendeeAsync(CreateAttendeeDTO dto);
        Task UpdateAttendeeAsync(UpdateAttendeeDTO dto);
        Task DeleteAttendeeAsync(Guid id);
        Task AddCompetenceToInstructorAsync(Guid instructorId, string competenceName);
    }
}
