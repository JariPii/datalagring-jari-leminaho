using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.CourseSessions
{
    public record AddInstructorToCourseSessionDTO
    {
        public Guid InstructorId { get; init; }
        public byte[] RowVersion { get; init; } = [];
    }
}
