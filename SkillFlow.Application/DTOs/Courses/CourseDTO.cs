namespace SkillFlow.Application.DTOs.Courses
{
    public record CourseDTO
    {
        public Guid Id { get; init; }

        public string CourseName { get; init; } = string.Empty;

        public string CourseDescription { get; init; } = string.Empty;

        public byte[] RowVersion { get; init; } = [];
    }
}
