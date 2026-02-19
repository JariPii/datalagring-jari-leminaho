using SkillFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.CourseSessions
{
   public record UpdateEnrollmentStatusDTO
    {
        public EnrollmentStatus NewStatus { get; init; }
        public byte[] RowVersion { get; init; } = default!;
    }
}
