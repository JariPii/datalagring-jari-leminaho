namespace SkillFlow.Domain.Entities.Competences
{
    public readonly record struct CompetenceId
    {
        public Guid Value { get; }

        public CompetenceId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Competence Id can not be empty", nameof(value));

            Value = value;
        }
        public static CompetenceId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
