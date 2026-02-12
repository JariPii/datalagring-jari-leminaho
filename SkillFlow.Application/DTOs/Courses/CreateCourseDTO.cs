using SkillFlow.Domain.Enums;

namespace SkillFlow.Application.DTOs.Courses
{
    public record CreateCourseDTO
    {
        public string CourseName { get; init; } = string.Empty;

        public string CourseDescription { get; init; } = string.Empty;

        public CourseType CourseType { get; init; }
    }
}
