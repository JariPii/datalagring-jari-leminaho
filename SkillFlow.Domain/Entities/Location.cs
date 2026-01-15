using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Location : BaseIdEntity<int>
    {
        public string LocationName { get; set; } = null!;
    }
}
