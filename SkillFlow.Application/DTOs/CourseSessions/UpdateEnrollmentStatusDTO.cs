using SkillFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.CourseSessions
{
    internal class UpdateEnrollmentStatusDTO
    {
        public EnrollmentStatus NewStatus { get; init; }
    }
}
