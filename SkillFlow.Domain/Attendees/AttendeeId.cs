using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public readonly record struct AttendeeId(Guid Value)
    {
        public static AttendeeId New() => new(Guid.NewGuid());
    }
}
