using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public class Competence : BaseEntity
    {
        public CompetenceId Id { get; private set; } = null!;
        public string Name { get; private set; } = null!;

        public Competence(CompetenceId id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            Id = id;
            Name = name;
        }

        protected Competence() { }

        public void UpdateCompetenceName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name is required", nameof(newName));

            if (Name == newName) return;

            Name = newName;
            UpdateTimeStamp();

        }

    }
}
