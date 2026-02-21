using FluentAssertions;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Tests.Fixtures;
using SkillFlow.Tests.TestData;
using Xunit;

namespace SkillFlow.Tests.Infrastructure.Repositories;

public sealed class CourseRepositoryTests : IDisposable
{
    private readonly SqliteInMemoryDbFixture _fx;
    private readonly SkillFlowDbContext _ctx;
    private readonly CourseRepository _repo;

    public CourseRepositoryTests()
    {
        _fx = new SqliteInMemoryDbFixture();
        _ctx = _fx.CreateContext();
        _repo = new CourseRepository(_ctx);
    }

    [Fact]
    public async Task SearchByNameAsync_returns_matching_courses()
    {
        await SkillFlowSeed.ResetAndSeedCoursesAsync(_ctx);

        var res = (await _repo.SearchByNameAsync("C#", CancellationToken.None)).ToList();

        res.Should().NotBeEmpty();
        res.Should().ContainSingle(c => c.CourseName.Value.Contains("C#", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ExistsByCourseName_returns_true_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedCoursesAsync(_ctx);

        var exists = await _repo.ExistsByCourseName(CourseName.Create("C# Grund"), CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetByCourseNameAsync_returns_course_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedCoursesAsync(_ctx);

        var course = await _repo.GetByCourseNameAsync(CourseName.Create("C# Grund"), CancellationToken.None);

        course.Should().NotBeNull();
        course!.CourseCode.Value.Should().Be("PRGBAS-001"); // om din formattering skiljer, justera denna rad
    }

    [Fact]
    public async Task ExistsByCourseCodeAsync_returns_true_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedCoursesAsync(_ctx);

        var exists = await _repo.ExistsByCourseCodeAsync(
            CourseCode.Create("PRG", CourseType.BAS, 1),
            CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetByCourseCodeAsync_returns_course_when_exists()
    {
        await SkillFlowSeed.ResetAndSeedCoursesAsync(_ctx);

        var course = await _repo.GetByCourseCodeAsync(
            CourseCode.Create("PRG", CourseType.BAS, 1),
            CancellationToken.None);

        course.Should().NotBeNull();
        course!.CourseName.Value.Should().Be("C# Grund");
    }

    [Fact]
    public async Task Remove_removes_course()
    {
        await SkillFlowSeed.ResetAndSeedCoursesAsync(_ctx);

        var course = await _repo.GetByCourseCodeAsync(
            CourseCode.Create("PRG", CourseType.BAS, 1),
            CancellationToken.None);

        course.Should().NotBeNull();

        _repo.Remove(course!);
        await _ctx.SaveChangesAsync();

        var again = await _repo.GetByCourseCodeAsync(
            CourseCode.Create("PRG", CourseType.BAS, 1),
            CancellationToken.None);

        again.Should().BeNull();
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _fx.Dispose();
    }
}