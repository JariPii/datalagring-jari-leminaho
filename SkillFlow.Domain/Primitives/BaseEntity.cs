using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Primitives
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }

        public void UpdateTimeStamp() => UpdatedAt = DateTime.UtcNow;
    }
}
