using FluentAssertions;
using Moq;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace SkillFlow.Tests.Presentation;

public class CourseEndpointsIntegrationTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public CourseEndpointsIntegrationTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _factory.CourseServiceMock.Reset();
    }

    // ---------------------------
    // helper: ProblemDetails (säker, ingen disposed-bugg)
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
    // GET /
    // ---------------------------

    [Fact]
    public async Task Get_courses_should_return_200_ok()
    {
        var client = _factory.CreateClient();

        // OBS: om din returtyp inte är IEnumerable<CourseDTO>, byt typen här
        _factory.CourseServiceMock
            .Setup(s => s.GetAllCoursesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<CourseDTO>>(Array.Empty<CourseDTO>()));

        var response = await client.GetAsync("/api/courses");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---------------------------
    // GET /search?searchTerm=...
    // ---------------------------

    [Fact]
    public async Task Get_courses_search_should_return_200_ok()
    {
        var client = _factory.CreateClient();
        var term = "c#";

        _factory.CourseServiceMock
            .Setup(s => s.SearchCoursesAsync(term, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<CourseDTO>>(Array.Empty<CourseDTO>()));

        var response = await client.GetAsync($"/api/courses/search?searchTerm={Uri.EscapeDataString(term)}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---------------------------
    // GET /{name}
    // ---------------------------

    [Fact]
    public async Task Get_course_by_name_should_return_404_problem_details_when_not_found()
    {
        var client = _factory.CreateClient();
        var name = "Missing course";

        // OBS: om CourseNotFoundException tar CourseName VO istället för string, byt raden till:
        // .ThrowsAsync(new CourseNotFoundException(CourseName.Create(name)));
        _factory.CourseServiceMock
            .Setup(s => s.GetCourseByNameAsync(name, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CourseNotFoundException(CourseName.Create(name)));

        var response = await client.GetAsync($"/api/courses/{Uri.EscapeDataString(name)}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(404);
        env.Problem.Title.Should().Be("Resource not found");
        env.ErrorCode.Should().Be(nameof(CourseNotFoundException));
        env.TraceId.Should().NotBeNullOrWhiteSpace();
    }

    // ---------------------------
    // POST /
    // ---------------------------

    [Fact]
    public async Task Post_courses_should_return_201_created_when_valid()
    {
        var client = _factory.CreateClient();

        var dto = new CreateCourseDTO
        {
            CourseName = "C# Fundamentals",
            CourseDescription = "Learn the basics of C# and .NET",
            CourseType = CourseType.BAS
        };

        var created = new CourseDTO
        {
            Id = Guid.NewGuid(),
            CourseName = dto.CourseName,
            CourseDescription = dto.CourseDescription,
            CourseType = dto.CourseType,
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _factory.CourseServiceMock
            .Setup(s => s.CreateCourseAsync(It.IsAny<CreateCourseDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var response = await client.PostAsync(
            "/api/courses",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().EndWith($"/api/courses/{created.Id}");
    }

    [Fact]
    public async Task Post_courses_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();

        var invalid = new CreateCourseDTO
        {
            CourseName = "",
            CourseDescription = "",
            CourseType = (CourseType)999 // invalid enum
        };

        var response = await client.PostAsync(
            "/api/courses",
            JsonContent.Create(invalid, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.CourseServiceMock.Verify(
            s => s.CreateCourseAsync(It.IsAny<CreateCourseDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Post_courses_should_return_409_problem_details_when_name_already_exists()
    {
        var client = _factory.CreateClient();

        var dto = new CreateCourseDTO
        {
            CourseName = "C# Fundamentals",
            CourseDescription = "Learn the basics of C# and .NET",
            CourseType = CourseType.BAS
        };

        // OBS: om exception tar string istället för VO, byt till new CourseNameAllreadyExistsException(dto.CourseName)
        _factory.CourseServiceMock
            .Setup(s => s.CreateCourseAsync(It.IsAny<CreateCourseDTO>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CourseNameAllreadyExistsException(CourseName.Create(dto.CourseName)));

        var response = await client.PostAsync(
            "/api/courses",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(409);
        env.Problem.Title.Should().Be("Resource conflict");
        env.ErrorCode.Should().Be(nameof(CourseNameAllreadyExistsException));
    }

    // ---------------------------
    // PATCH /{id:guid}
    // ---------------------------

    [Fact]
    public async Task Patch_courses_should_return_200_ok_when_valid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var dto = new UpdateCourseDTO
        {
            CourseName = "Updated name",
            CourseDescription = null,
            RowVersion = new byte[] { 9, 9, 9 }
        };

        var updated = new CourseDTO
        {
            Id = id,
            CourseName = dto.CourseName!,
            CourseDescription = "Old description",
            CourseType = CourseType.BAS,
            RowVersion = new byte[] { 1, 1, 1 }
        };

        _factory.CourseServiceMock
            .Setup(s => s.UpdateCourseAsync(id, It.IsAny<UpdateCourseDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var response = await client.PatchAsync(
            $"/api/courses/{id}",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Patch_courses_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var invalid = new UpdateCourseDTO
        {
            CourseName = "",                  // invalid when not null
            CourseDescription = null,
            RowVersion = Array.Empty<byte>()  // required
        };

        var response = await client.PatchAsync(
            $"/api/courses/{id}",
            JsonContent.Create(invalid, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.CourseServiceMock.Verify(
            s => s.UpdateCourseAsync(It.IsAny<Guid>(), It.IsAny<UpdateCourseDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ---------------------------
    // DELETE /{id:guid}
    // ---------------------------

    [Fact]
    public async Task Delete_courses_should_return_204_no_content_when_success()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _factory.CourseServiceMock
            .Setup(s => s.DeleteCourseAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await client.DeleteAsync($"/api/courses/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_courses_should_return_422_problem_details_when_course_in_use()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var name = "C# Fundamentals";

        _factory.CourseServiceMock
            .Setup(s => s.DeleteCourseAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CourseInUseException(
                CourseName.Create(name)
            ));

        var response = await client.DeleteAsync($"/api/courses/{id}");

        response.StatusCode.Should().Be((HttpStatusCode)422);

        var env = await ReadProblemAsync(response);

        env.Problem.Status.Should().Be(422);
        env.Problem.Title.Should().Be("Dependency Conflict");
        env.ErrorCode.Should().Be(nameof(CourseInUseException));
    }
}