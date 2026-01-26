using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Locations
{
    public record UpdateLocationDTO
    {
        public Guid Id { get; init; }

        public string? Name { get; init; }
    }
}
