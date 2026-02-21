using FluentAssertions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Tests.Fixtures;
using SkillFlow.Tests.TestData;
using Xunit;

namespace SkillFlow.Tests.Infrastructure.Repositories;

public sealed class BaseRepositoryUpdateTests : IDisposable
{
    private readonly SqliteInMemoryDbFixture _fx;
    private readonly SkillFlowDbContext _ctx;

    public BaseRepositoryUpdateTests()
    {
        _fx = new SqliteInMemoryDbFixture();
        _ctx = _fx.CreateContext();
    }

    [Fact]
    public async Task UpdateAsync_throws_when_rowVersion_missing()
    {
        await SkillFlowSeed.ResetAndSeedLocationsAsync(_ctx);

        var repo = new LocationRepository(_ctx);
        var loc = await repo.GetByLocationNameAsync(LocationName.Create("Stockholm"), CancellationToken.None);

        loc.Should().NotBeNull();

        var act = () => repo.UpdateAsync(loc!, rowVersion: null, CancellationToken.None);

        act.Should().ThrowAsync<Exception>();
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _fx.Dispose();
    }
}