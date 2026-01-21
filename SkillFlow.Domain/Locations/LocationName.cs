using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Locations
{
    public readonly record struct LocationName
    {
        public string Value { get; }

        private LocationName(string value)
        {
            Value = value;
        }

        public static LocationName Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            value = value.Trim();

            return new LocationName(value);
        }

        public override string ToString() => Value;
    }
}
