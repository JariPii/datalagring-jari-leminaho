using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public readonly record struct CompetenceId(Guid Value)
    {
        public static CompetenceId New() => new(Guid.NewGuid());
    }
}
