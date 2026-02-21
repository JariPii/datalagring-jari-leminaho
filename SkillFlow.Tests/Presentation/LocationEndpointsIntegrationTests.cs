using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Moq;
using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Exceptions;
using Xunit;

namespace SkillFlow.Tests.Presentation;

public class LocationEndpointsIntegrationTests : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory _factory;

    public LocationEndpointsIntegrationTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _factory.LocationServiceMock.Reset();
    }

    // ---------------------------
    // helper: ProblemDetails (säker)
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
    public async Task Get_locations_should_return_200_ok()
    {
        var client = _factory.CreateClient();

        _factory.LocationServiceMock
            .Setup(s => s.GetAllLocationsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<LocationDTO>>(Array.Empty<LocationDTO>()));

        var response = await client.GetAsync("/api/locations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---------------------------
    // GET /{id}
    // ---------------------------

    [Fact]
    public async Task Get_location_by_id_should_return_404_problem_details_when_not_found()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        // Om din LocationNotFoundException kräver LocationId VO (troligt), använd new LocationId(id)
        _factory.LocationServiceMock
            .Setup(s => s.GetLocationByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new LocationNotFoundException(new LocationId(id)));

        var response = await client.GetAsync($"/api/locations/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(404);
        env.Problem.Title.Should().Be("Resource not found");
        env.ErrorCode.Should().Be(nameof(LocationNotFoundException));
        env.TraceId.Should().NotBeNullOrWhiteSpace();
    }

    // ---------------------------
    // GET /search?searchTerm=...
    // ---------------------------

    [Fact]
    public async Task Get_locations_search_should_return_200_ok()
    {
        var client = _factory.CreateClient();
        var term = "stock";

        _factory.LocationServiceMock
            .Setup(s => s.SearchLocationsAsync(term, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<LocationDTO>>(Array.Empty<LocationDTO>()));

        var response = await client.GetAsync($"/api/locations/search?searchTerm={Uri.EscapeDataString(term)}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---------------------------
    // POST /
    // ---------------------------

    [Fact]
    public async Task Post_locations_should_return_201_created_when_valid()
    {
        var client = _factory.CreateClient();

        var dto = new CreateLocationDTO
        {
            Name = "Stockholm"
        };

        var created = new LocationDTO
        {
            Id = Guid.NewGuid(),
            LocationName = dto.Name,
            RowVersion = new byte[] { 1, 2, 3 }
        };

        _factory.LocationServiceMock
            .Setup(s => s.CreateLocationAsync(It.IsAny<CreateLocationDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var response = await client.PostAsJsonAsync("/api/locations", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().EndWith($"/api/locations/{created.Id}");
    }

    [Fact]
    public async Task Post_locations_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();

        var invalid = new CreateLocationDTO { Name = "" };

        var response = await client.PostAsJsonAsync("/api/locations", invalid);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.LocationServiceMock.Verify(
            s => s.CreateLocationAsync(It.IsAny<CreateLocationDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Post_locations_should_return_409_problem_details_when_name_already_exists()
    {
        var client = _factory.CreateClient();

        var dto = new CreateLocationDTO { Name = "Stockholm" };

        // Om exception tar LocationName VO: LocationName.Create(dto.Name)
        _factory.LocationServiceMock
            .Setup(s => s.CreateLocationAsync(It.IsAny<CreateLocationDTO>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new LocationNameAllreadyExistsException(LocationName.Create(dto.Name)));

        var response = await client.PostAsJsonAsync("/api/locations", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var env = await ReadProblemAsync(response);
        env.Problem.Status.Should().Be(409);
        env.Problem.Title.Should().Be("Resource conflict");
        env.ErrorCode.Should().Be(nameof(LocationNameAllreadyExistsException));
    }

    // ---------------------------
    // PATCH /{id}
    // ---------------------------

    [Fact]
    public async Task Patch_locations_should_return_200_ok_when_valid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var dto = new UpdateLocationDTO
        {
            Name = "Gothenburg",
            RowVersion = new byte[] { 9, 9, 9 }
        };

        var updated = new LocationDTO
        {
            Id = id,
            LocationName = dto.Name!,
            RowVersion = new byte[] { 1, 1, 1 }
        };

        _factory.LocationServiceMock
            .Setup(s => s.UpdateLocationAsync(id, It.IsAny<UpdateLocationDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var response = await client.PatchAsJsonAsync($"/api/locations/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Patch_locations_should_return_400_bad_request_when_body_invalid()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        // Name empty (when not null) + RowVersion required
        var invalid = new UpdateLocationDTO
        {
            Name = "",
            RowVersion = Array.Empty<byte>()
        };

        var response = await client.PatchAsJsonAsync($"/api/locations/{id}", invalid);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _factory.LocationServiceMock.Verify(
            s => s.UpdateLocationAsync(It.IsAny<Guid>(), It.IsAny<UpdateLocationDTO>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ---------------------------
    // DELETE /{id}
    // ---------------------------

    [Fact]
    public async Task Delete_locations_should_return_204_no_content_when_success()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        _factory.LocationServiceMock
            .Setup(s => s.DeleteLocationAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await client.DeleteAsync($"/api/locations/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_locations_should_return_422_problem_details_when_location_in_use()
    {
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        var name = "Stockholm";

        _factory.LocationServiceMock
            .Setup(s => s.DeleteLocationAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new LocationInUseException(
                LocationName.Create(name)
            ));

        var response = await client.DeleteAsync($"/api/locations/{id}");

        response.StatusCode.Should().Be((HttpStatusCode)422);

        var env = await ReadProblemAsync(response);

        env.Problem.Status.Should().Be(422);
        env.Problem.Title.Should().Be("Dependency Conflict");
        env.ErrorCode.Should().Be(nameof(LocationInUseException));
    }

    [Fact]
    public async Task Get_locations_should_return_500_problem_details_when_unhandled_exception()
    {
        var client = _factory.CreateClient();

        _factory.LocationServiceMock
            .Setup(s => s.GetAllLocationsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        var response = await client.GetAsync("/api/locations");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var env = await ReadProblemAsync(response);

        env.Problem.Status.Should().Be(500);
        env.Problem.Title.Should().Be("Internal Server Error");
        env.ErrorCode.Should().Be("Exception");
    }
}
