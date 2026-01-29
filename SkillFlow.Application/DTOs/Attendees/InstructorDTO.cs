using SkillFlow.Application.DTOs.Courses;
using System.Text.Json.Serialization;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record InstructorDTO : AttendeeDTO
    {
        [JsonPropertyOrder(100)]
        public List<CompetenceDTO> Competences { get; init; } = [];
    }
}
