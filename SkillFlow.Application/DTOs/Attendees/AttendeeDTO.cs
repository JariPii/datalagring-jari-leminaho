using SkillFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillFlow.Application.DTOs.Attendees
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(StudentDTO), "student")]
    [JsonDerivedType(typeof(InstructorDTO), "instructor")]
    public record AttendeeDTO
    {
        [JsonPropertyOrder(-11)]
        public Guid Id { get; init; }

        public string Email { get; init; } = string.Empty;

        [JsonPropertyOrder(-9)]
        public string FirstName { get; init; } = string.Empty;

        [JsonPropertyOrder(-8)]
        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        [JsonPropertyOrder(-10)]
        public Role Role { get; init; }
        public byte[] RowVersion { get; init; } = [];

    }
}
