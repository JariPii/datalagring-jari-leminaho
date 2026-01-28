namespace SkillFlow.Domain.Entities.Courses
{
    public readonly record struct CourseId
    {
        public Guid Value { get; }

        public CourseId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Course id can not be empty", nameof(value));
        }
        public static CourseId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
