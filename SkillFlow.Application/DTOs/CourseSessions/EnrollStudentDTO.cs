using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record EnrollStudentDTO
    {
        public Guid StudentId { get; init; }
    }
}
