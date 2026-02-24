namespace SkillFlow.Application.DTOs.Locations
{
    public record UpdateLocationDTO
    {
        public string? LocationName { get; init; }

        public byte[] RowVersion { get; init; } = default!;
    }
}
