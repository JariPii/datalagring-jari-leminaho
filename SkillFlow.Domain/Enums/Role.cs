using System.ComponentModel.DataAnnotations;

namespace SkillFlow.Domain.Enums
{
    public enum Role
    {
        [Display(Name = "Student")]
        Student,

        [Display(Name = "Instructor")]
        Instructor
    }
}
