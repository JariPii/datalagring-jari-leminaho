using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Primitives;
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
            var cleanValue = value.NormalizeName();

            if (cleanValue.Length == 0)
                throw new InvalidCompetenceNameException($"A competence name is required");

            if (cleanValue.Length > MaxLength)
                throw new InvalidCompetenceNameException($"The competence name cannot contain more than {MaxLength} characters");

            return new CompetenceName(cleanValue);
        }

        public override string ToString() => Value;

    }
}
