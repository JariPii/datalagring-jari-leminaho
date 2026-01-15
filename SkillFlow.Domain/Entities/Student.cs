using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Student
    {
        public Guid AttendeeId { get; set; }
        public Attendee Attendee { get; set; } = null!;
    }
}
