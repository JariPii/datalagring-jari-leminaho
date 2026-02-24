namespace SkillFlow.Application.DTOs.Locations
{
    public record CreateLocationDTO
    {
        public string LocationName { get; init; } = string.Empty;
    }
}
