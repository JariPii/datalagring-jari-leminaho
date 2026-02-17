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
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            var trimmedLocationName = value.Trim();

            if (trimmedLocationName.Length > MaxLength)
                throw new ArgumentException($"Location name cannot exceed {MaxLength} characters", nameof(value));

            return new LocationName(trimmedLocationName);
        }

        public override string ToString() => Value;
    }
}
