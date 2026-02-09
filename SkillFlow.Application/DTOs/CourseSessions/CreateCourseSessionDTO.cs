namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record CreateCourseSessionDTO
    {
        //public Guid CourseId { get; init; }

        //public Guid LocationId { get; init; }
        public string CourseCode { get; init; } = string.Empty;
        public string LocationName { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public int Capacity { get; init; }
        public List<Guid> InstructorIds { get; init; } = [];
    }
}
