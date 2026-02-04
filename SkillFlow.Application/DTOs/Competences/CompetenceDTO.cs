
using SkillFlow.Application.DTOs.Attendees;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SkillFlow.Application.DTOs.Competences
{
    public record CompetenceDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
