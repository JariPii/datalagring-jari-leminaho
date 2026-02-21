using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace SkillFlow.Tests.Presentation;

public class AttendeeEndpointsIntegrationTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public AttendeeEndpointsIntegrationTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _factory.AttendeeServiceMock.Reset();
    }

    [Fact]
    public async Task Post_attendees_should_return_201_created_when_valid()
    {
        var client = _factory.CreateClient();

        var dto = new CreateAttendeeDTO
        {
            Email = "test@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = null,
            Role = Role.Student
        };

        var created = new StudentDTO
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber
        };

        _factory.AttendeeServiceMock
            .Setup(s => s.CreateAttendeeAsync(It.IsAny<CreateAttendeeDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Viktigt: skicka request med string-enums
        var response = await client.PostAsync(
            "/api/attendees",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().EndWith($"/api/attendees/{created.Id}");

        var body = await response.Content.ReadFromJsonAsync<StudentDTO>(JsonOptions);
        body.Should().NotBeNull();
        body!.Id.Should().Be(created.Id);
        body.Email.Should().Be(created.Email);
    }

    [Fact]
    public async Task Post_attendees_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();

        var invalid = new CreateAttendeeDTO
        {
            Email = "",
            FirstName = "",
            LastName = "",
            PhoneNumber = null,
            Role = Role.Student
        };

        var response = await client.PostAsync(
            "/api/attendees",
            JsonContent.Create(invalid, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.AttendeeServiceMock.Verify(
            s => s.CreateAttendeeAsync(It.IsAny<CreateAttendeeDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Post_attendees_should_return_409_when_email_already_exists()
    {
        var client = _factory.CreateClient();

        var dto = new CreateAttendeeDTO
        {
            Email = "test@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = null,
            Role = Role.Student
        };

        _factory.AttendeeServiceMock
            .Setup(s => s.CreateAttendeeAsync(It.IsAny<CreateAttendeeDTO>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EmailAlreadyExistsException(
                Email.Create(dto.Email)
            ));

        var response = await client.PostAsync(
            "/api/attendees",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Get_attendees_by_id_should_return_200_ok_when_found()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var found = new StudentDTO
        {
            Id = id,
            Email = "found@gmail.com",
            FirstName = "Ada",
            LastName = "Lovelace",
            PhoneNumber = null
        };

        _factory.AttendeeServiceMock
            .Setup(s => s.GetAttendeeByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(found);

        var response = await client.GetAsync($"/api/attendees/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<StudentDTO>(JsonOptions);
        body.Should().NotBeNull();
        body!.Id.Should().Be(id);
        body.Email.Should().Be(found.Email);
    }

    [Fact]
    public async Task Get_attendees_by_id_should_return_404_when_not_found()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _factory.AttendeeServiceMock
            .Setup(s => s.GetAttendeeByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AttendeeNotFoundException(
                new AttendeeId(id)
            ));

        var response = await client.GetAsync($"/api/attendees/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_attendees_by_email_should_return_404_when_not_found()
    {
        var client = _factory.CreateClient();
        var email = "missing@gmail.com";

        _factory.AttendeeServiceMock
            .Setup(s => s.GetAttendeeByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AttendeeNotFoundException(
                Email.Create(email)
            ));

        var response = await client.GetAsync($"/api/attendees/by-email?email={Uri.EscapeDataString(email)}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Patch_attendees_should_return_200_ok_when_valid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var dto = new UpdateAttendeeDTO
        {
            Email = "new@gmail.com",
            FirstName = "Ada",
            LastName = "Lovelace",
            PhoneNumber = "+46701234567",
            RowVersion = new byte[] { 1, 2, 3 }
        };

        var updated = new StudentDTO
        {
            Id = id,
            Email = dto.Email!,
            FirstName = dto.FirstName!,
            LastName = dto.LastName!,
            PhoneNumber = dto.PhoneNumber
        };

        _factory.AttendeeServiceMock
            .Setup(s => s.UpdateAttendeeAsync(id, It.IsAny<UpdateAttendeeDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var response = await client.PatchAsync(
            $"/api/attendees/{id}",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<StudentDTO>(JsonOptions);
        body.Should().NotBeNull();
        body!.Email.Should().Be(updated.Email);
    }

    [Fact]
    public async Task Patch_attendees_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        // RowVersion krävs i validatorn => invalid
        var invalid = new UpdateAttendeeDTO
        {
            Email = null,
            FirstName = null,
            LastName = null,
            PhoneNumber = null,
            RowVersion = Array.Empty<byte>()
        };

        var response = await client.PatchAsync(
            $"/api/attendees/{id}",
            JsonContent.Create(invalid, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.AttendeeServiceMock.Verify(
            s => s.UpdateAttendeeAsync(It.IsAny<Guid>(), It.IsAny<UpdateAttendeeDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_attendees_should_return_204_no_content_when_success()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _factory.AttendeeServiceMock
            .Setup(s => s.DeleteAttendeeAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await client.DeleteAsync($"/api/attendees/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Post_attendees_should_return_409_problem_details_when_email_already_exists()
    {
        var client = _factory.CreateClient();

        var dto = new CreateAttendeeDTO
        {
            Email = "test@gmail.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = null,
            Role = Role.Student
        };

        _factory.AttendeeServiceMock
            .Setup(s => s.CreateAttendeeAsync(It.IsAny<CreateAttendeeDTO>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EmailAlreadyExistsException(Email.Create(dto.Email)));

        var response = await client.PostAsync(
            "/api/attendees",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var env = await ReadProblemAsync(response);

        env.Problem.Status.Should().Be(409);
        env.Problem.Title.Should().Be("Resource conflict");
        env.Problem.Detail.Should().Contain("test@gmail.com");

        env.ErrorCode.Should().Be(nameof(EmailAlreadyExistsException));
        env.TraceId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Get_attendees_by_id_should_return_404_problem_details_when_not_found()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _factory.AttendeeServiceMock
            .Setup(s => s.GetAttendeeByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AttendeeNotFoundException(new AttendeeId(id)));

        var response = await client.GetAsync($"/api/attendees/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var env = await ReadProblemAsync(response);

        env.Problem.Status.Should().Be(404);
        env.Problem.Title.Should().Be("Resource not found");
        env.Problem.Detail.Should().Contain(id.ToString());

        env.ErrorCode.Should().Be(nameof(AttendeeNotFoundException));
        env.TraceId.Should().NotBeNullOrWhiteSpace();

        // timestamp kan vara null om den serialiseras som objekt, men oftast string
        // så denna assert kan vara "soft":
        // env.Timestamp.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_competence_should_return_400_problem_details_on_business_rule_violation()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var dto = new AddCompetenceDTO("C#", new byte[] { 1, 2, 3 });

        _factory.AttendeeServiceMock
            .Setup(s => s.AddCompetenceToInstructorAsync(id, dto.CompetenceName, dto.RowVersion, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidCompetenceNameException("Bad name"));

        var response = await client.PostAsync(
            $"/api/attendees/{id}/competences",
            JsonContent.Create(dto, options: JsonOptions));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var env = await ReadProblemAsync(response);

        env.Problem.Status.Should().Be(400);
        env.Problem.Title.Should().Be("Business Rule Violation");
        env.ErrorCode.Should().Be(nameof(InvalidCompetenceNameException));
        env.TraceId.Should().NotBeNullOrWhiteSpace();
    }

    private sealed record ProblemEnvelope(
     ProblemDetails Problem,
     string? ErrorCode,
     string? TraceId,
     DateTimeOffset? Timestamp
 );

    private static async Task<ProblemEnvelope> ReadProblemAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        var problem = new ProblemDetails
        {
            Status = root.TryGetProperty("status", out var status) ? status.GetInt32() : null,
            Title = root.TryGetProperty("title", out var title) ? title.GetString() : null,
            Detail = root.TryGetProperty("detail", out var detail) ? detail.GetString() : null,
            Type = root.TryGetProperty("type", out var type) ? type.GetString() : null,
            Instance = root.TryGetProperty("instance", out var instance) ? instance.GetString() : null,
        };

        string? errorCode =
            root.TryGetProperty("errorCode", out var ec) ? ec.GetString() : null;

        string? traceId =
            root.TryGetProperty("traceId", out var tid) ? tid.GetString() : null;

        DateTimeOffset? timestamp = null;
        if (root.TryGetProperty("timestamp", out var ts))
        {
            // timestamp kan vara string eller annat beroende på serializer
            if (ts.ValueKind == JsonValueKind.String && DateTimeOffset.TryParse(ts.GetString(), out var parsed))
                timestamp = parsed;
        }

        return new ProblemEnvelope(problem, errorCode, traceId, timestamp);
    }
}