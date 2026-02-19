namespace SkillFlow.Application.DTOs.Attendees
{
    public record UpdateAttendeeDTO
    {
        public string? Email { get; init; }

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? PhoneNumber { get; init; }

        public byte[] RowVersion { get; init; } = default!;
    }
}
