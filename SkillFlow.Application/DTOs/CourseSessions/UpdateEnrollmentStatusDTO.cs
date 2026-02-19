using SkillFlow.Domain.Enums;

namespace SkillFlow.Application.DTOs.CourseSessions
{
   public record UpdateEnrollmentStatusDTO
    {
        public EnrollmentStatus NewStatus { get; init; }
        public byte[] RowVersion { get; init; } = default!;
    }
}
