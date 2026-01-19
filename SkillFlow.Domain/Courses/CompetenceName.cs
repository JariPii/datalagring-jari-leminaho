using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public readonly record struct CompetenceName
    {
        public string Value { get; }

        public CompetenceName(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));


            Value = value.Trim();
        }

        public override string ToString() => Value;
    }
}
