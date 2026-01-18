using SkillFlow.Domain.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.CourseSessions
{
    public readonly record struct EnrollmentId(Guid Value)
    {
        public static EnrollmentId New() => new(Guid.NewGuid());
    }
}
