using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.DTOs.Locations;

namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record CourseSessionDTO
    {
        public Guid Id { get; init; }

        public required CourseDTO Course { get; init; }

        public string CourseCode { get; init; } = string.Empty;

        public required LocationDTO Location { get; init; }

        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public int Capacity { get; init; }

        public List<AttendeeDTO> Instructors { get; init; } = [];

        public int ApprovedEnrollmentsCount { get; init; }

        public byte[] RowVersion { get; init; } = [];
    }
}
