namespace SkillFlow.Application.DTOs.Locations
{
    public record LocationDTO
    {
        public Guid Id { get; init; }

        public string LocationName { get; init; } = string.Empty;
    }
}
