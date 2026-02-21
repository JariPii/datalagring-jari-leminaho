using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Moq;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Exceptions;
using Xunit;

namespace SkillFlow.Tests.Presentation;

public class CompetenceEndpointsIntegrationTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;

    public CompetenceEndpointsIntegrationTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _factory.CompetenceServiceMock.Reset();
    }

    // ---------------------------
    // helper (ProblemDetails)
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
    public async Task Get_competences_should_return_200_ok()
    {
        var client = _factory.CreateClient();

        _factory.CompetenceServiceMock
            .Setup(s => s.GetAllCompetencesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<CompetenceDetailsDTO>>(Array.Empty<CompetenceDetailsDTO>()));

        var response = await client.GetAsync("/api/competences");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---------------------------
    // GET /{id}
    // ---------------------------

    [Fact]
    public async Task Get_competence_by_id_should_return_404_problem_details_when_not_found()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _factory.CompetenceServiceMock
            .Setup(s => s.GetCompetenceDetailsAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CompetenceNotFoundException(new CompetenceId(id))); // VO ctor

        var response = await client.GetAsync($"/api/competences/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(404);
        env.Problem.Title.Should().Be("Resource not found");
        env.ErrorCode.Should().Be(nameof(CompetenceNotFoundException));
        env.TraceId.Should().NotBeNullOrWhiteSpace();
    }

    // ---------------------------
    // POST /
    // ---------------------------

    [Fact]
    public async Task Post_competences_should_return_201_created_when_valid()
    {
        var client = _factory.CreateClient();

        var dto = new CreateCompetenceDTO { Name = "C#" };

        var created = new CompetenceDTO
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _factory.CompetenceServiceMock
            .Setup(s => s.CreateCompetenceAsync(It.IsAny<CreateCompetenceDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var response = await client.PostAsJsonAsync("/api/competences", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().EndWith($"/api/competences/{created.Id}");
    }

    [Fact]
    public async Task Post_competences_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();

        var invalid = new CreateCompetenceDTO { Name = "" }; // validator: NotEmpty + MaxLength(50)

        var response = await client.PostAsJsonAsync("/api/competences", invalid);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.CompetenceServiceMock.Verify(
            s => s.CreateCompetenceAsync(It.IsAny<CreateCompetenceDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Post_competences_should_return_409_problem_details_when_name_already_exists()
    {
        var client = _factory.CreateClient();

        var dto = new CreateCompetenceDTO { Name = "C#" };

        _factory.CompetenceServiceMock
            .Setup(s => s.CreateCompetenceAsync(It.IsAny<CreateCompetenceDTO>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CompetenceNameAllreadyExistsException(CompetenceName.Create(dto.Name)));

        var response = await client.PostAsJsonAsync("/api/competences", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(409);
        env.Problem.Title.Should().Be("Resource conflict");
        env.ErrorCode.Should().Be(nameof(CompetenceNameAllreadyExistsException));
    }

    // ---------------------------
    // PATCH /{id}
    // ---------------------------

    [Fact]
    public async Task Patch_competences_should_return_200_ok_when_valid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var dto = new UpdateCompetenceDTO
        {
            Name = "C# Advanced",
            RowVersion = new byte[] { 9, 9, 9 }
        };

        var updated = new CompetenceDTO
        {
            Id = id,
            Name = dto.Name!,
            RowVersion = new byte[] { 1, 1, 1 }
        };

        _factory.CompetenceServiceMock
            .Setup(s => s.UpdateCompetenceAsync(id, It.IsAny<UpdateCompetenceDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var response = await client.PatchAsJsonAsync($"/api/competences/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Patch_competences_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var invalid = new UpdateCompetenceDTO
        {
            Name = "",                     // Name not empty when not null
            RowVersion = Array.Empty<byte>() // RowVersion required
        };

        var response = await client.PatchAsJsonAsync($"/api/competences/{id}", invalid);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.CompetenceServiceMock.Verify(
            s => s.UpdateCompetenceAsync(It.IsAny<Guid>(), It.IsAny<UpdateCompetenceDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ---------------------------
    // DELETE /{id}
    // ---------------------------

    [Fact]
    public async Task Delete_competences_should_return_204_no_content_when_success()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _factory.CompetenceServiceMock
            .Setup(s => s.DeleteCompetenceAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await client.DeleteAsync($"/api/competences/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}