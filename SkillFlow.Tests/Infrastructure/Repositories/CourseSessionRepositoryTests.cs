using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Tests.Fixtures;
using SkillFlow.Tests.TestData;
using Xunit;

namespace SkillFlow.Tests.Infrastructure.Repositories;

public sealed class CourseSessionRepositoryTests : IDisposable
{
    private readonly SqliteInMemoryDbFixture _fx;
    private readonly SkillFlowDbContext _ctx;
    private readonly CourseSessionRepository _repo;

    public CourseSessionRepositoryTests()
    {
        _fx = new SqliteInMemoryDbFixture();
        _ctx = _fx.CreateContext();
        _repo = new CourseSessionRepository(_ctx);
    }

    [Fact]
    public async Task GetAllAsync_includes_course_location_enrollments_instructors()
    {
        await SkillFlowSeed.ResetAndSeedCourseSessionsAsync(_ctx);

        var all = (await _repo.GetAllAsync(CancellationToken.None)).ToList();

        all.Should().NotBeEmpty();
        all.Should().AllSatisfy(s =>
        {
            s.Course.Should().NotBeNull();
            s.Location.Should().NotBeNull();
            s.Enrollments.Should().NotBeNull();
            s.Instructors.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task SearchByNameAsync_returns_sessions_and_includes_course_location_instructors()
    {
        await SkillFlowSeed.ResetAndSeedCourseSessionsAsync(_ctx);

        var res = (await _repo.SearchAsync("Stockholm", CancellationToken.None)).ToList();

        res.Should().NotBeEmpty();
        res.Should().AllSatisfy(s =>
        {
            s.Course.Should().NotBeNull();
            s.Location.Should().NotBeNull();
            s.Instructors.Should().NotBeNull();
        });

        res.Any(s => s.Location.LocationName.Value == "Stockholm").Should().BeTrue();
    }

    [Fact]
    public async Task GetEnrollmentsBySessionIdAsync_returns_enrollments_with_student()
    {
        await SkillFlowSeed.ResetAndSeedCourseSessionsAsync(_ctx);

        var sessionId = await _ctx.CourseSessions.Select(s => s.Id).SingleAsync();

        var enrollments = (await _repo.GetEnrollmentsBySessionIdAsync(sessionId, CancellationToken.None)).ToList();

        enrollments.Should().NotBeEmpty();
        enrollments.Should().AllSatisfy(e => e.Student.Should().NotBeNull());
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _fx.Dispose();
    }
}