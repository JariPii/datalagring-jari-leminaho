using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Locations
{
    public readonly record struct LocationName
    {
        public string Value { get; }

        public const int MaxLength = 50;

        private LocationName(string value)
        {
            Value = value;
        }

        public static LocationName Create(string value)
        {

            var cleanLocationName = value.NormalizeName();

            if (cleanLocationName.Length == 0)
                throw new InvalidLocationNameException("Location name is required");

            if (cleanLocationName.Length > MaxLength)
                throw new InvalidLocationNameException($"Location name cannot exceed {MaxLength} characters");

            return new LocationName(cleanLocationName);
        }

        public override string ToString() => Value;
    }
}
