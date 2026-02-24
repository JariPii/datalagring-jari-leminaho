using SkillFlow.Domain.Enums;

namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record EnrollmentDetailsDTO
    {
        public Guid CourseSessionId { get; init; }
        public string CourseName { get; init; } = string.Empty;
        public string CourseCode { get; init; } = string.Empty;
        public string LocationName { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        public EnrollmentStatus Status { get; init; }
        public DateTime EnrolledAt { get; init; }
        public byte[] CourseSessionRowVersion { get; init; } = default!;
    }
}
