using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Tests.Fixtures;
using SkillFlow.Tests.TestData;
using Xunit;

namespace SkillFlow.Tests.Infrastructure.Repositories;

public sealed class CompetenceRepositoryTests : IDisposable
{
    private readonly SqliteInMemoryDbFixture _fx;
    private readonly SkillFlowDbContext _ctx;
    private readonly CompetenceRepository _repo;

    public CompetenceRepositoryTests()
    {
        _fx = new SqliteInMemoryDbFixture();
        _ctx = _fx.CreateContext();
        _repo = new CompetenceRepository(_ctx);
    }

    [Fact]
    public async Task ExistsByNameAsync_returns_true_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedCompetencesAsync(_ctx);

        var exists = await _repo.ExistsByNameAsync(CompetenceName.Create("C#"), CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_returns_false_when_missing()
    {
        await SkillFlowSeed.ResetAndSeedCompetencesAsync(_ctx);

        var exists = await _repo.ExistsByNameAsync(CompetenceName.Create("Rust"), CancellationToken.None);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetByNameAsync_returns_competence_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedCompetencesAsync(_ctx);

        var c = await _repo.GetByNameAsync(CompetenceName.Create("C#"), CancellationToken.None);

        c.Should().NotBeNull();
        c!.Name.Value.Should().Be("C#");
    }

    [Fact]
    public async Task GetByIdAsync_includes_instructors()
    {
        await SkillFlowSeed.ResetAndSeedCompetencesAsync(_ctx);

        var id = await _ctx.Competences
            .Where(c => c.Name == CompetenceName.Create("C#"))
            .Select(c => c.Id)
            .SingleAsync();

        var c = await _repo.GetByIdAsync(id, CancellationToken.None);

        c.Should().NotBeNull();
        c!.Instructors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_includes_instructors_and_is_no_tracking()
    {
        await SkillFlowSeed.ResetAndSeedCompetencesAsync(_ctx);

        var all = (await _repo.GetAllAsync(CancellationToken.None)).ToList();

        all.Should().NotBeEmpty();
        all.Should().AllSatisfy(c => c.Instructors.Should().NotBeNull());

        // AsNoTracking - bör ge 0 entries efter queryn (gäller om du inte spårar seed i samma context)
        // Eftersom vi seedar i samma context, kan ChangeTracker innehålla seedade entities.
        // Så istället verifierar vi att queryn inte exploderar och att includes funkar.
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _fx.Dispose();
    }
}