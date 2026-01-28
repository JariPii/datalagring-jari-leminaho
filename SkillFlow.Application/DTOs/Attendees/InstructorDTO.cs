using SkillFlow.Application.DTOs.Courses;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record InstructorDTO : AttendeeDTO
    {
        public List<CompetenceDTO> Competences { get; init; } = [];
    }
}
