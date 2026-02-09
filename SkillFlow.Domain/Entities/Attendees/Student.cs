using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Domain.Entities.Attendees
{
    public class Student : Attendee
    {
        internal Student(AttendeeId id, Email email, AttendeeName name, PhoneNumber? phoneNumber) : base(id, email, name, phoneNumber, Role.Student) { }

        private Student() : base() { }
    }

}
