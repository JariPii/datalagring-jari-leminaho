namespace SkillFlow.Application.DTOs.Courses
{
    public record UpdateCourseDTO
    {
        public Guid Id { get; init; }

        public string? CourseName { get; init; }

        public string? CourseDescription { get; init; }
    }
}
