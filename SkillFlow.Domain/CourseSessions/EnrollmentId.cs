using SkillFlow.Domain.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.CourseSessions
{
    public record EnrollmentId(Guid Value)
    {
        public static EnrollmentId New() => new(Guid.NewGuid());
    }
}
