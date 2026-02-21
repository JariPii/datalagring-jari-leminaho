using Microsoft.EntityFrameworkCore;
using SkillFlow.Infrastructure;

namespace SkillFlow.Tests.Fixtures;

public sealed class TestSkillFlowDbContext : SkillFlowDbContext
{
    public TestSkillFlowDbContext(DbContextOptions<SkillFlowDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SQLite kan inte auto-generera rowversion som SQL Server gör.
        // Gör RowVersion nullable i testmodellen så EnsureCreated + inserts funkar.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var rowVersionProp = entityType.FindProperty("RowVersion");
            if (rowVersionProp is null) continue;

            modelBuilder.Entity(entityType.ClrType)
                .Property(rowVersionProp.ClrType, "RowVersion")
                .IsRequired(false);
        }
    }
}