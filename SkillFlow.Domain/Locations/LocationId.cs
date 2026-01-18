using SkillFlow.Domain.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Locations
{
    public record LocationId(Guid Value)
    {
        public static LocationId New() => new(Guid.NewGuid());
    }
}
