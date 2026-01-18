using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public readonly record struct CourseId(Guid Value)
    {
        public static CourseId New() => new(Guid.NewGuid());
    }
}
