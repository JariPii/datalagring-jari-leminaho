namespace SkillFlow.Application.DTOs.Courses
{
    public record UpdateCourseDTO
    {
        public string? CourseName { get; init; }

        public string? CourseDescription { get; init; }

        public byte[] RowVersion { get; init; } = default!;
    }
}
