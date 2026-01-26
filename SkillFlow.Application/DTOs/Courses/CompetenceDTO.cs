using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Courses
{
    public record CompetenceDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
