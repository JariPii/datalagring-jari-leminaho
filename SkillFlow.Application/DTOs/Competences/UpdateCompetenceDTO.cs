using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Competences
{
    public record UpdateCompetenceDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
