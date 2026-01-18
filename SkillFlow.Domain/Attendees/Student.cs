using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public class Student : Attendee
    {
        public Student(AttendeeId id, Email email, string firstName, string lastName) : base(id, email, firstName, lastName, Role.Student) { }

        private Student() : base() { }
    }

}
