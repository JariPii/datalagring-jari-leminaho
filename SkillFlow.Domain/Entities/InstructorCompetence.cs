using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class InstructorCompetence
    {
        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public int CompetenceId { get; set; }
        public Competence Competence { get; set; } = null!;
    }
}
