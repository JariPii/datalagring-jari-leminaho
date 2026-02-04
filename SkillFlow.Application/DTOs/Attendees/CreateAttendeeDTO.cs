using SkillFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record CreateAttendeeDTO
    {
        [Required]
        public string Email { get; init; } = string.Empty;

        [Required]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        [Required]
        public Role Role { get; init; }
    }
}
