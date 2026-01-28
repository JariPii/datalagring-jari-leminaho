using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Domain.Entities.Attendees
{
    public class Student : Attendee
    {
        public Student(AttendeeId id, Email email, AttendeeName name, PhoneNumber? phoneNumber) : base(id, email, name, Role.Student, phoneNumber) { }

        private Student() : base() { }
    }

}
