namespace SkillFlow.Application.DTOs.Competences
{
    public record CompetenceDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public byte[] RowVersion { get; init; } = default!;
    }
}
