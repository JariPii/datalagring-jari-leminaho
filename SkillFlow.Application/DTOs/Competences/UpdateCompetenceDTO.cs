using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Competences
{
    public record UpdateCompetenceDTO
    {
        public string Name { get; init; } = string.Empty;
        public byte[] RowVersion { get; init; } = default!;
    }
}
