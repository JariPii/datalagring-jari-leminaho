namespace SkillFlow.Application.DTOs.Locations
{
    public record UpdateLocationDTO
    {
        public Guid Id { get; init; }

        public string? Name { get; init; }

        public byte[] RowVersion { get; init; } = [];
    }
}
