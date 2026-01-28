namespace SkillFlow.Domain.Entities.Attendees
{
    public readonly record struct AttendeeId
    {
        public Guid Value { get; }

        public AttendeeId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("AttendeeId can not be empty", nameof(value));

            Value = value;
        }

        public static AttendeeId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
