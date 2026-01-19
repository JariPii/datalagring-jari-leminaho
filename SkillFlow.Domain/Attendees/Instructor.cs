using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public class Instructor : Attendee
    {
        public Instructor(AttendeeId id, Email email, AttendeeName name, PhoneNumber? phoneNumber) : base(id, email, name, Role.Instructor, phoneNumber) { }

        private Instructor () : base () { }

    }
}
