using SkillFlow.Domain.Enums;
using System.Text.Json.Serialization;

namespace SkillFlow.Application.DTOs.Attendees
{
    [JsonDerivedType(typeof(AttendeeDTO))]
    [JsonDerivedType(typeof(InstructorDTO))]
    public record AttendeeDTO
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        public Role Role { get; init; }

    }
}
