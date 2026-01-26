using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record UpdateAttendeeDTO
    {
        public Guid Id { get; init; }

        public string? Email { get; init; }

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? PhoneNumber { get; init; }
    }
}
