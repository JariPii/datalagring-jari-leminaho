using SkillFlow.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public class Instructor : Attendee
    {
        private readonly List<Competence> _competences = new();
        public Instructor(AttendeeId id, Email email, AttendeeName name, PhoneNumber? phoneNumber)
            : base(id, email, name, Role.Instructor, phoneNumber) { }

        private Instructor () : base () { }

        public virtual IReadOnlyCollection<Competence> Competences => _competences.AsReadOnly();

        public void AddCompetence(Competence competence)
        {
            if (!_competences.Any(c => c.Id == competence.Id))
            {
                _competences.Add(competence);
                UpdateTimeStamp();
            }
        }

    }
}
