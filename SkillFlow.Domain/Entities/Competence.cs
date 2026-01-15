using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Competence : BaseIdEntity<int>
    {
        public string Name { get; set; } = null!;

        public ICollection<InstructorCompetence> InstructorCompetences { get; set; } = [];
    }
}
