namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record EnrollStudentDTO
    {
        public Guid StudentId { get; init; }
        public byte[] RowVersion { get; init; } = default!;
    }
}
