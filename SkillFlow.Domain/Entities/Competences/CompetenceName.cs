using SkillFlow.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Entities.Competences
{
    public readonly partial record struct CompetenceName
    {
        public const int MaxLength = 50;
        public string Value { get; }

        private CompetenceName(string value) => Value = value;

        public static CompetenceName Create(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new InvalidCompetenceNameException("A competence name is required");

            var generatedValue = MyRegEx().Replace(value.Trim(), " ");

            if (generatedValue.Length > MaxLength)
                throw new InvalidCompetenceNameException($"The competence name cannot contain more than {MaxLength} characters");

            return new CompetenceName(generatedValue);

        }

        public override string ToString() => Value;

        [GeneratedRegex(@"\s+")]
        private static partial Regex MyRegEx();

    }
}
