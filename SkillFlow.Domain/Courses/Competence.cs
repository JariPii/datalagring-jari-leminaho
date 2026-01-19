using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public class Competence : BaseEntity
    {
        public CompetenceId Id { get; private set; }
        public string Name { get; private set; } = null!;

        public Competence(CompetenceId id, string name)
        {
            if (id.Value == Guid.Empty)
                throw new ArgumentException("Competence Id can not be empty", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            Id = id;
            Name = name.NormalizeName();
        }

        protected Competence() { }

        public void UpdateCompetenceName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name is required", nameof(newName));

            var normalizedName = newName.NormalizeName();

            if (Name == normalizedName) return;

            Name = normalizedName;
            UpdateTimeStamp();

        }

    }
}
