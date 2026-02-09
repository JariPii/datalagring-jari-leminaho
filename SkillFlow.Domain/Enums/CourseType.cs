using System.ComponentModel.DataAnnotations;

namespace SkillFlow.Domain.Enums
{
    public enum CourseType
    {
        [Display(Name = "Grundkurs")]
        GRD,
        [Display(Name = "Försdjupningskurs")]
        FDJ
    }
}
