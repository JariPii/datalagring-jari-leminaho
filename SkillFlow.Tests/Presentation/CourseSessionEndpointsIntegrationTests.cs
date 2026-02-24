using FluentAssertions;
using Moq;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace SkillFlow.Tests.Presentation;

public class CourseSessionEndpointsIntegrationTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public CourseSessionEndpointsIntegrationTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _factory.CourseSessionServiceMock.Reset();
    }

    // ---------------------------
    // helpers
    // ---------------------------

    private sealed record ProblemEnvelope(
        Microsoft.AspNetCore.Mvc.ProblemDetails Problem,
        string? ErrorCode,
        string? TraceId
    );

    private static async Task<ProblemEnvelope> ReadProblemAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = root.TryGetProperty("status", out var status) ? status.GetInt32() : null,
            Title = root.TryGetProperty("title", out var title) ? title.GetString() : null,
            Detail = root.TryGetProperty("detail", out var detail) ? detail.GetString() : null,
            Instance = root.TryGetProperty("instance", out var instance) ? instance.GetString() : null,
            Type = root.TryGetProperty("type", out var type) ? type.GetString() : null,
        };

        var errorCode = root.TryGetProperty("errorCode", out var ec) ? ec.GetString() : null;
        var traceId = root.TryGetProperty("traceId", out var tid) ? tid.GetString() : null;

        return new ProblemEnvelope(problem, errorCode, traceId);
    }

    // ---------------------------
    // tests
    // ---------------------------

    [Fact]
    public async Task Post_courseSessions_should_return_201_created_when_valid()
    {
        var client = _factory.CreateClient();

        var dto = new CreateCourseSessionDTO
        {
            CourseCode = "MTHBAS-010",
            LocationName = "Stockholm",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Capacity = 10,
            InstructorIds = new List<Guid> { Guid.NewGuid() }
        };

        // return-typen kan vara annan i ditt projekt; byt vid behov
        var created = new CourseSessionDTO
        {
            Id = Guid.NewGuid(),

            Course = new CourseDTO
            {
                Id = Guid.NewGuid(),
                CourseName = "C# Fundamentals",
                CourseDescription = "Learn the basics of C# and .NET",
                CourseType = CourseType.BAS // byt om din enum heter annorlunda
            },

            CourseCode = dto.CourseCode,

            Location = new LocationDTO
            {
                Id = Guid.NewGuid(),
                LocationName = dto.LocationName,
                RowVersion = new byte[] { 1, 1, 1 }
            },

            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Capacity = dto.Capacity,

            Instructors = new List<AttendeeDTO>(),

            ApprovedEnrollmentsCount = 0,
            RowVersion = new byte[] { 2, 2, 2 }
        };

        _factory.CourseSessionServiceMock
            .Setup(s => s.CreateCourseSessionAsync(It.IsAny<CreateCourseSessionDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var response = await client.PostAsync(
            "/api/courseSessions",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().EndWith($"/api/courseSessions/{created.Id}");
    }

    [Fact]
    public async Task Post_courseSessions_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();

        var invalid = new CreateCourseSessionDTO
        {
            CourseCode = "",
            LocationName = "",
            StartDate = default,
            EndDate = default,
            Capacity = 0,
            InstructorIds = new List<Guid>()
        };

        var response = await client.PostAsync(
            "/api/courseSessions",
            JsonContent.Create(invalid, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.CourseSessionServiceMock.Verify(
            s => s.CreateCourseSessionAsync(It.IsAny<CreateCourseSessionDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Get_courseSessions_by_id_should_return_404_problem_details_when_not_found()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        // Välj den not found-exception du har i domänen (den finns i handlern)
        _factory.CourseSessionServiceMock
            .Setup(s => s.GetCourseSessionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CourseSessionNotFoundException(new CourseSessionId(id))); // <-- byt ctor om din kräver VO

        var response = await client.GetAsync($"/api/courseSessions/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(404);
        env.Problem.Title.Should().Be("Resource not found");
        env.ErrorCode.Should().Be(nameof(CourseSessionNotFoundException));
        env.TraceId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Post_enrollments_should_return_400_problem_details_when_session_is_full()
    {
        var client = _factory.CreateClient();
        var sessionId = Guid.NewGuid();

        var dto = new EnrollStudentDTO
        {
            StudentId = Guid.NewGuid(),
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _factory.CourseSessionServiceMock
            .Setup(s => s.EnrollStudentAsync(sessionId, dto.StudentId, dto.RowVersion, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CourseSessionFullException(10));

        var response = await client.PostAsync(
            $"/api/courseSessions/{sessionId}/enrollments",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(400);
        env.Problem.Title.Should().Be("Business Rule Violation");
        env.ErrorCode.Should().Be(nameof(CourseSessionFullException));
    }

    [Fact]
    public async Task Put_enrollment_status_should_return_409_problem_details_on_concurrency_exception()
    {
        var client = _factory.CreateClient();
        var sessionId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var dto = new UpdateEnrollmentStatusDTO
        {
            NewStatus = EnrollmentStatus.Approved,
            RowVersion = new byte[] { 9, 9, 9 }
        };

        _factory.CourseSessionServiceMock
            .Setup(s => s.SetEnrollmentStatusAsync(
                sessionId,
                studentId,
                dto.NewStatus,
                dto.RowVersion,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConcurrencyException());

        var response = await client.PutAsync(
            $"/api/courseSessions/{sessionId}/enrollment/{studentId}/status",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var env = await ReadProblemAsync(response);

        env.Problem.Status.Should().Be(409);

        // Behåll om du verkligen mappar Title exakt så här:
        env.Problem.Title.Should().Be("Resource conflict");

        // Om du lägger errorCode i extensions istället: anpassa ReadProblemAsync.
        env.ErrorCode.Should().Be(nameof(ConcurrencyException));

        _factory.CourseSessionServiceMock.Verify(s => s.SetEnrollmentStatusAsync(
            sessionId,
            studentId,
            dto.NewStatus,
            dto.RowVersion,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Post_instructors_should_return_200_ok_when_success()
    {
        var client = _factory.CreateClient();
        var sessionId = Guid.NewGuid();

        var dto = new AddInstructorToCourseSessionDTO
        {
            InstructorId = Guid.NewGuid(),
            RowVersion = new byte[] { 1, 1, 1 }
        };

        _factory.CourseSessionServiceMock
            .Setup(s => s.AddInstructorToCourseSessionAsync(sessionId, dto.InstructorId, dto.RowVersion, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await client.PostAsync(
            $"/api/courseSessions/{sessionId}/instructors",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}