using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Infrastructure;

namespace SkillFlow.Tests.TestData;

public static class SkillFlowSeed
{
    // ============================================================
    // RESET DATABASE (safe for SQLite in-memory testing)
    // ============================================================
    private static async Task ResetDatabaseAsync(SkillFlowDbContext ctx, CancellationToken ct)
    {
        await ctx.Database.EnsureDeletedAsync(ct);
        await ctx.Database.EnsureCreatedAsync(ct);
    }

    // ============================================================
    // BASIC: Attendees + Competences
    // Used by:
    // - AttendeeRepositoryTests
    // - CompetenceRepositoryTests
    // ============================================================
    public static async Task ResetAndSeedBasicAsync(
        SkillFlowDbContext ctx,
        CancellationToken ct = default)
    {
        await ResetDatabaseAsync(ctx, ct);

        var csharp = Competence.Create(CompetenceName.Create("C#"));
        var dotnet = Competence.Create(CompetenceName.Create(".NET"));

        var instructor = (Instructor)Attendee.CreateInstructor(
            Email.Create("instructor@skillflow.test"),
            AttendeeName.Create("Alice", "Teacher"),
            phoneNumber: null);

        instructor.AddCompetence(csharp);
        instructor.AddCompetence(dotnet);

        var student = (Student)Attendee.CreateStudent(
            Email.Create("student@skillflow.test"),
            AttendeeName.Create("Bob", "Student"),
            phoneNumber: null);

        ctx.AddRange(
            csharp,
            dotnet,
            instructor,
            student);

        await ctx.SaveChangesAsync(ct);
    }

    // ============================================================
    // COMPETENCES + instructors (include tests)
    // Used by:
    // - CompetenceRepositoryTests
    // ============================================================
    public static async Task ResetAndSeedCompetencesAsync(
        SkillFlowDbContext ctx,
        CancellationToken ct = default)
    {
        await ResetDatabaseAsync(ctx, ct);

        var csharp = Competence.Create(CompetenceName.Create("C#"));
        var dotnet = Competence.Create(CompetenceName.Create(".NET"));

        var instructor1 = (Instructor)Attendee.CreateInstructor(
            Email.Create("alice@skillflow.test"),
            AttendeeName.Create("Alice", "Teacher"),
            phoneNumber: null);

        var instructor2 = (Instructor)Attendee.CreateInstructor(
            Email.Create("eve@skillflow.test"),
            AttendeeName.Create("Eve", "Trainer"),
            phoneNumber: null);

        instructor1.AddCompetence(csharp);
        instructor1.AddCompetence(dotnet);

        instructor2.AddCompetence(csharp);

        ctx.AddRange(
            csharp,
            dotnet,
            instructor1,
            instructor2);

        await ctx.SaveChangesAsync(ct);
    }

    // ============================================================
    // LOCATIONS
    // Used by:
    // - LocationRepositoryTests
    // ============================================================
    public static async Task ResetAndSeedLocationsAsync(
        SkillFlowDbContext ctx,
        CancellationToken ct = default)
    {
        await ResetDatabaseAsync(ctx, ct);

        var stockholm = Location.Create(
            LocationName.Create("Stockholm"));

        var gothenburg = Location.Create(
            LocationName.Create("Gothenburg"));

        ctx.AddRange(
            stockholm,
            gothenburg);

        await ctx.SaveChangesAsync(ct);
    }

    // ============================================================
    // COURSES
    // Used by:
    // - CourseRepositoryTests
    // ============================================================
    public static async Task ResetAndSeedCoursesAsync(
        SkillFlowDbContext ctx,
        CancellationToken ct = default)
    {
        await ResetDatabaseAsync(ctx, ct);

        var course1 = Course.Create(
            CourseCode.Create("PRG", CourseType.BAS, 1),
            CourseName.Create("C# Grund"),
            CourseDescription.Create("Intro till C#"));

        var course2 = Course.Create(
            CourseCode.Create("PRG", CourseType.BAS, 2),
            CourseName.Create("ASP.NET API"),
            CourseDescription.Create("Bygg APIer"));

        ctx.AddRange(
            course1,
            course2);

        await ctx.SaveChangesAsync(ct);
    }

    // ============================================================
    // COURSE SESSIONS + ENROLLMENT
    // Used by:
    // - CourseSessionRepositoryTests
    // ============================================================
    public static async Task ResetAndSeedCourseSessionsAsync(
        SkillFlowDbContext ctx,
        CancellationToken ct = default)
    {
        await ResetDatabaseAsync(ctx, ct);

        var course = Course.Create(
            CourseCode.Create("PRG", CourseType.BAS, 1),
            CourseName.Create("C# Grund"),
            CourseDescription.Create("Intro till C#"));

        var location = Location.Create(
            LocationName.Create("Stockholm"));

        var instructor = (Instructor)Attendee.CreateInstructor(
            Email.Create("instructor@skillflow.test"),
            AttendeeName.Create("Alice", "Teacher"),
            phoneNumber: null);

        var student = (Student)Attendee.CreateStudent(
            Email.Create("student@skillflow.test"),
            AttendeeName.Create("Bob", "Student"),
            phoneNumber: null);

        var session = CourseSession.Create(
            CourseSessionId.New(),
            course.Id,
            course.CourseCode,
            new DateTime(2030, 1, 10),
            new DateTime(2030, 1, 12),
            capacity: 10,
            location.Id);

        // domain rules
        session.AddInstructor(instructor);
        session.AddStudent(student);

        ctx.AddRange(
            course,
            location,
            instructor,
            student,
            session);

        await ctx.SaveChangesAsync(ct);
    }
}