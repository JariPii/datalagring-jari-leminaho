using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Competences
{
    public record CreateCompetenceDTO
    {
        public string Name { get; init; } = string.Empty;
    }
}
