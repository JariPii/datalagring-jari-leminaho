using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.CourseSessions
{
    public readonly record struct CourseSessionId(Guid Value)
    {
        public static CourseSessionId New() => new(Guid.NewGuid());
    }
}
