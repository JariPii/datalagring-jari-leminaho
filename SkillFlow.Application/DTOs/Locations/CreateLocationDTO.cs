using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Locations
{
    public record CreateLocationDTO
    {
        public string Name { get; init; } = string.Empty;
    }
}
