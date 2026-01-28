namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record UpdateCourseSessionDTO
    {
        public Guid Id { get; init; }
        public Guid? CourseId { get; init; }

        public Guid? LocationId { get; init; }

        public DateTime? StartDate { get; init; }

        public DateTime? EndDate { get; init; }

        public int? Capacity { get; init; }
    }
}
