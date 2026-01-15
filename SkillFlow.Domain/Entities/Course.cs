using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Course : BaseIdEntity<int>
    {
        public string CourseCode { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string CourseDescription { get; set; } = null!;
    }
}
