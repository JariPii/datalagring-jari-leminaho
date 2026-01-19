using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Locations
{
    public readonly record struct LocationName
    {
        public string Value { get; }       

        public LocationName(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            Value = value.Trim();
        }

        public override string ToString() => Value;
    }
}
