using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public class Student : Attendee
    {
        public Student(AttendeeId id, Email email, AttendeeName name, PhoneNumber? phoneNumber) : base(id, email, name, Role.Student, phoneNumber) { }

        private Student() : base() { }
    }

}
