using SkillFlow.Domain.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Locations
{
    public readonly record struct LocationId
    {
        public Guid Value { get; }

        public LocationId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Location Id can not be empty", nameof(value));

            Value = value;
        }

        public static LocationId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
