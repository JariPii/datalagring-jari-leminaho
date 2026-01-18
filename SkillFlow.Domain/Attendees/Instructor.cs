using SkillFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public class Instructor : Attendee
    {
        public Instructor(AttendeeId id, string email, string firstName, string lastName) : base(id, email, firstName, lastName, Role.Instructor) { }

        private Instructor () : base () { }

    }
}
