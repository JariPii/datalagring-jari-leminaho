using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.CourseSessions
{
    public readonly record struct CourseSessionId
    {
        public Guid Value { get; }

        public CourseSessionId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("CourseSession id can not be mepty", nameof(value));

            Value = value;
        }
        public static CourseSessionId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
