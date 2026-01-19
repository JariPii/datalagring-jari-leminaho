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
        public CompetenceName Name { get; private set; }

        public Competence(CompetenceId id, CompetenceName name)
        {
            Id = id;
            Name = name;
        }

        protected Competence() { }

        public void UpdateCompetenceName(CompetenceName newName)
        {
            if (Name == newName) return;            

            Name = newName;
            UpdateTimeStamp();

        }

    }
}
