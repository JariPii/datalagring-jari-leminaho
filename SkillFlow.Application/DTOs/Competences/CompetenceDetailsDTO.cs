using SkillFlow.Application.DTOs.Attendees;
using System.Text.Json.Serialization;

namespace SkillFlow.Application.DTOs.Competences
{
    public record CompetenceDetailsDTO : CompetenceDTO
    {
        [JsonPropertyOrder(100)]
        public List<AttendeeDTO> Instructors { get; init; } = [];
    }
}
