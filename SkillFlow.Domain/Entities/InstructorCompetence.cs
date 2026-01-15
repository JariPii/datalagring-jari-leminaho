using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class InstructorCompetence
    {
        public int Instructor_Id { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public int Competence_Id { get; set; }
        public Competence Competence { get; set; } = null!;
    }
}
