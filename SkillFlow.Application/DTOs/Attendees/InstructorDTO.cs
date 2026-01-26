using SkillFlow.Application.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record InstructorDTO : AttendeeDTO
    {
        public List<CompetenceDTO> Competences { get; init; } = new();
    }
}
