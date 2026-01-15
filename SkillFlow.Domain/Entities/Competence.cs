using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Competence
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<InstructorCompetence> InstructorCompetences { get; set; }
    }
}
