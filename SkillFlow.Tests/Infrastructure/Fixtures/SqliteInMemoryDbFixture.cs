using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SkillFlow.Infrastructure;

namespace SkillFlow.Tests.Fixtures;

public sealed class SqliteInMemoryDbFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public DbContextOptions<SkillFlowDbContext> Options { get; }

    public SqliteInMemoryDbFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<SkillFlowDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        using var ctx = CreateContext();
        ctx.Database.EnsureCreated();
    }

    // OBS: returnera TestSkillFlowDbContext här
    public SkillFlowDbContext CreateContext() => new TestSkillFlowDbContext(Options);

    public void Dispose() => _connection.Dispose();
}