using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record CreateCourseSessionDTO
    {
        public Guid CourseId { get; init; }

        public Guid LocationId { get; init; }

        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public int Capacity { get; init; }
    }
}
