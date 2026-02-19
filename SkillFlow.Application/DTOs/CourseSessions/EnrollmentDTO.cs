using SkillFlow.Domain.Enums;

namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record EnrollmentDTO
    {
        public Guid Id { get; init; }
        public Guid StudentId { get; init; }
        public string StudentName { get; init; } = string.Empty;
        public EnrollmentStatus Status { get; init; }
        public DateTime EnrolledAt { get; init; }
    }
}
