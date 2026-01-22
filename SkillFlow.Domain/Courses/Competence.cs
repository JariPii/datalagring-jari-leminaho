using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public class Competence : BaseEntity
    {
        private readonly List<Instructor> _instructors = new();
        public CompetenceId Id { get; private set; }
        public CompetenceName Name { get; private set; }

        public Competence(CompetenceId id, CompetenceName name)
        {
            Id = id;
            Name = name;
        }

        protected Competence() { }

        public virtual IReadOnlyCollection<Instructor> Instructors => _instructors.AsReadOnly();

        public void UpdateCompetenceName(CompetenceName newName)
        {
            if (Name == newName) return;            

            Name = newName;
            UpdateTimeStamp();

        }

        public void AddInstructor(Instructor instructor)
        {
            ArgumentNullException.ThrowIfNull(instructor);

            if(!_instructors.Any(i => i.Id == instructor.Id))
            {
                _instructors.Add(instructor);
                UpdateTimeStamp();
            }
        }

    }
}
