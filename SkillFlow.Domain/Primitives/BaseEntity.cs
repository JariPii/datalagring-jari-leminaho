namespace SkillFlow.Domain.Primitives
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; protected set; } = default!;
        public byte[] RowVersion { get; private set; } = [];
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }

        public void UpdateTimeStamp() => UpdatedAt = DateTime.UtcNow;
    }
}
