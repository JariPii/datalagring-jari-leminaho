using System.ComponentModel.DataAnnotations;

namespace SkillFlow.Domain.Enums
{
    public enum CourseType
    {
        [Display(Name = "Basic")]
        BAS,
        [Display(Name = "Intermediate")]
        INT,
        [Display(Name =  "Advanced")]
        ADV,
        [Display(Name = "Expert")]
        EXP
    }
}
