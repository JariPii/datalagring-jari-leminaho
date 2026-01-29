using SkillFlow.Domain.Enums;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record CreateAttendeeDTO
    {
        public string Email { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        public Role Role { get; init; }
    }
}
