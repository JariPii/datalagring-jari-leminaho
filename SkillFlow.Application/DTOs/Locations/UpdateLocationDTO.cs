namespace SkillFlow.Application.DTOs.Locations
{
    public record UpdateLocationDTO
    {
        public string? Name { get; init; }

        public byte[] RowVersion { get; init; } = default!;
    }
}
