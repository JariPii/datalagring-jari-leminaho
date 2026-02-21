using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Enums;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Tests.Fixtures;
using SkillFlow.Tests.TestData;
using Xunit;

namespace SkillFlow.Tests.Repositories;

public sealed class AttendeeRepositoryTests : IDisposable
{
    private readonly SqliteInMemoryDbFixture _fx;
    private readonly SkillFlowDbContext _ctx;
    private readonly AttendeeRepository _repo;

    public AttendeeRepositoryTests()
    {
        _fx = new SqliteInMemoryDbFixture();
        _ctx = _fx.CreateContext();
        _repo = new AttendeeRepository(_ctx);
    }

    [Fact]
    public async Task ExistsByEmailAsync_returns_true_when_email_exists()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var exists = await _repo.ExistsByEmailAsync(
            Email.Create("instructor@skillflow.test"),
            CancellationToken.None);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_returns_false_when_email_does_not_exist()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var exists = await _repo.ExistsByEmailAsync(
            Email.Create("missing@skillflow.test"),
            CancellationToken.None);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_returns_instructors_and_students_and_loads_instructor_competences()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var all = (await _repo.GetAllAsync(CancellationToken.None)).ToList();

        all.Any(a => a is Instructor).Should().BeTrue();
        all.Any(a => a is Student).Should().BeTrue();

        var instructor = all.OfType<Instructor>().Single();
        instructor.Competences.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_for_instructor_loads_competences()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var instructorId = await _ctx.Instructors.Select(i => i.Id).SingleAsync();

        var attendee = await _repo.GetByIdAsync(instructorId, CancellationToken.None);

        var instructor = attendee.Should().BeOfType<Instructor>().Subject;
        instructor.Competences.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_for_student_returns_student()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var studentId = await _ctx.Students.Select(s => s.Id).SingleAsync();

        var attendee = await _repo.GetByIdAsync(studentId, CancellationToken.None);

        attendee.Should().BeOfType<Student>();
    }

    [Fact]
    public async Task GetInstructorsByCompetenceAsync_returns_empty_when_blank()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var res = await _repo.GetInstructorsByCompetenceAsync("   ", CancellationToken.None);

        res.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInstructorsByCompetenceAsync_returns_matching_instructors()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var res = (await _repo.GetInstructorsByCompetenceAsync("C#", CancellationToken.None)).ToList();

        res.Should().NotBeEmpty();
        res.Should().OnlyContain(i => i.Role == Role.Instructor);

        // (valfritt) sanity check på namn-mappning från SQL-aliasserna
        res.Any(i => i.Name.FirstName == "Alice").Should().BeTrue();
    }

    [Fact]
    public async Task SearchByNameAsync_returns_empty_when_blank()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var res = await _repo.SearchByNameAsync("", CancellationToken.None);

        res.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchByNameAsync_finds_by_first_or_last_name()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var res1 = (await _repo.SearchByNameAsync("Alice", CancellationToken.None)).ToList();
        res1.Should().NotBeEmpty();

        var res2 = (await _repo.SearchByNameAsync("Student", CancellationToken.None)).ToList();
        res2.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllInstructorsAsync_returns_instructors_with_competences()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var instructors = (await _repo.GetAllInstructorsAsync(CancellationToken.None)).ToList();

        instructors.Should().NotBeEmpty();
        instructors.Should().OnlyContain(i => i.Role == Role.Instructor);
        instructors.All(i => i.Competences.Any()).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllStudentsAsync_returns_only_students()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var students = (await _repo.GetAllStudentsAsync(CancellationToken.None)).ToList();

        students.Should().NotBeEmpty();
        students.Should().OnlyContain(s => s.Role == Role.Student);
    }

    [Fact]
    public async Task GetCompetenceByNameAsync_returns_correct_competence()
    {
        await SkillFlowSeed.ResetAndSeedBasicAsync(_ctx);

        var competence = await _repo.GetCompetenceByNameAsync(
            CompetenceName.Create("C#"),
            CancellationToken.None);

        competence.Should().NotBeNull();
        competence!.Name.Value.Should().Be("C#");
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _fx.Dispose();
    }
}