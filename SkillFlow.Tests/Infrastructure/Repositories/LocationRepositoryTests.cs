using FluentAssertions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Tests.Fixtures;
using SkillFlow.Tests.TestData;
using Xunit;

namespace SkillFlow.Tests.Infrastructure.Repositories;

public sealed class LocationRepositoryTests : IDisposable
{
    private readonly SqliteInMemoryDbFixture _fx;
    private readonly SkillFlowDbContext _ctx;
    private readonly LocationRepository _repo;

    public LocationRepositoryTests()
    {
        _fx = new SqliteInMemoryDbFixture();
        _ctx = _fx.CreateContext();
        _repo = new LocationRepository(_ctx);
    }

    [Fact]
    public async Task ExistsByNameAsync_returns_true_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedLocationsAsync(_ctx);

        var exists = await _repo.ExistsByNameAsync(LocationName.Create("Stockholm"), CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetByLocationNameAsync_returns_location_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedLocationsAsync(_ctx);

        var loc = await _repo.GetByLocationNameAsync(LocationName.Create("Stockholm"), CancellationToken.None);

        loc.Should().NotBeNull();
        loc!.LocationName.Value.Should().Be("Stockholm");
    }

    [Fact]
    public async Task SearchByNameAsync_returns_matching_locations()
    {
        await SkillFlowSeed.ResetAndSeedLocationsAsync(_ctx);

        var res = (await _repo.SearchByNameAsync("Stock", CancellationToken.None)).ToList();

        res.Should().NotBeEmpty();
        res.Any(l => l.LocationName.Value.Contains("Stock", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task Remove_removes_location()
    {
        await SkillFlowSeed.ResetAndSeedLocationsAsync(_ctx);

        var loc = await _repo.GetByLocationNameAsync(LocationName.Create("Stockholm"), CancellationToken.None);
        loc.Should().NotBeNull();

        _repo.Remove(loc!);
        await _ctx.SaveChangesAsync();

        var again = await _repo.GetByLocationNameAsync(LocationName.Create("Stockholm"), CancellationToken.None);
        again.Should().BeNull();
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _fx.Dispose();
    }
}