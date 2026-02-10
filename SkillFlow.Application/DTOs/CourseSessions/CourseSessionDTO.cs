namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record CourseSessionDTO
    {
        public Guid Id { get; init; }

        public string Course { get; init; } = string.Empty;

        public string CourseCode { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;

        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public int Capacity { get; init; }

        public List<string> Instructors { get; init; } = [];

        public int ApprovedEnrollmentsCount { get; init; }

        public byte[] RowVersion { get; init; } = [];
    }
}
