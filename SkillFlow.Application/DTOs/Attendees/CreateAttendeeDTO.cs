using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record CreateAttendeeDTO
    {
        public string Email { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        public string Role { get; init; } = string.Empty;
    }
}
