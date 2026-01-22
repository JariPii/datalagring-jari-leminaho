using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Locations
{
    public readonly record struct LocationName
    {
        public string Value { get; }
        public const int MaxLength = 150;

        private LocationName(string value)
        {
            Value = value;
        }

        public static LocationName Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            var trimmedLocationName = value.Trim();

            if (trimmedLocationName.Length > MaxLength)
                throw new ArgumentException($"Location name cannot exceed {MaxLength} characters", nameof(value));

            return new LocationName(trimmedLocationName);
        }

        public override string ToString() => Value;
    }
}
