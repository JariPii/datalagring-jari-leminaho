using SkillFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Attendee : BaseIdEntity<Guid>
    {
        
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public Role Role { get; set; }

        public Student? Student { get; set; }
        public Instructor? Instructor { get; set; }
    }
}
