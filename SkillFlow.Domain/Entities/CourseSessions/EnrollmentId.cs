namespace SkillFlow.Domain.Entities.CourseSessions
{
    public readonly record struct EnrollmentId
    {
        public Guid Value { get; }

        public EnrollmentId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Enrollment id can not be empty", nameof(value));

            Value = value;
        }
        public static EnrollmentId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
