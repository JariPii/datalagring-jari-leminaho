using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SkillFlow.Presentation.Filters;
using System.Text.Json;
using Xunit;

namespace SkillFlow.Tests.Presentation;

public class ValidationFilterTests
{
    public sealed record DummyDto(string Name);

    [Fact]
    public async Task InvokeAsync_should_return_400_problem_when_dto_is_missing()
    {
        var http = new DefaultHttpContext();
        http.Response.Body = new MemoryStream();

        http.RequestServices = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var ctx = new FakeEndpointFilterInvocationContext(http, new object?[] { "not-a-dto" });

        var filter = new ValidationFilter<DummyDto>();

        var result = await filter.InvokeAsync(ctx, _ => ValueTask.FromResult<object?>("ok"));

        result.Should().NotBeNull();

        await ((IResult)result!).ExecuteAsync(http);

        http.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        http.Response.Body.Position = 0;
        var json = await new StreamReader(http.Response.Body).ReadToEndAsync();

        json.Should().Contain("Invalid request body");
        json.Should().Contain("Request body is missin or could not be parsed");
    }

    [Fact]
    public async Task InvokeAsync_should_call_next_when_validator_is_missing()
    {
        var http = new DefaultHttpContext();

        http.RequestServices = new ServiceCollection().BuildServiceProvider();

        var dto = new DummyDto("x");
        var ctx = new FakeEndpointFilterInvocationContext(http, new object?[] { dto });

        var called = false;
        ValueTask<object?> Next(EndpointFilterInvocationContext _)
        {
            called = true;
            return ValueTask.FromResult<object?>("ok");
        }

        var filter = new ValidationFilter<DummyDto>();

        var result = await filter.InvokeAsync(ctx, Next);

        called.Should().BeTrue();
        result.Should().Be("ok");
    }

    [Fact]
    public async Task InvokeAsync_should_return_validation_problem_when_invalid()
    {
        var http = new DefaultHttpContext();
        http.Response.Body = new MemoryStream();

        var dto = new DummyDto("");

        var validator = new Mock<IValidator<DummyDto>>();
        validator.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure(nameof(DummyDto.Name), "Name is required")
            }));

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(validator.Object);
        http.RequestServices = services.BuildServiceProvider();

        var ctx = new FakeEndpointFilterInvocationContext(http, new object?[] { dto });

        var filter = new ValidationFilter<DummyDto>();

        var result = await filter.InvokeAsync(ctx, _ => ValueTask.FromResult<object?>("ok"));

        result.Should().NotBeNull();

        await ((IResult)result!).ExecuteAsync(http);

        http.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        http.Response.Body.Position = 0;
        var json = await new StreamReader(http.Response.Body).ReadToEndAsync();

        json.Should().Contain("Validation failed");
        json.Should().Contain("Name is required");
    }

    [Fact]
    public async Task InvokeAsync_should_call_next_when_valid()
    {
        var http = new DefaultHttpContext();

        var dto = new DummyDto("valid");

        var validator = new Mock<IValidator<DummyDto>>();
        validator.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var services = new ServiceCollection();
        services.AddSingleton(validator.Object);
        http.RequestServices = services.BuildServiceProvider();

        var ctx = new FakeEndpointFilterInvocationContext(http, new object?[] { dto });

        var called = false;
        ValueTask<object?> Next(EndpointFilterInvocationContext _)
        {
            called = true;
            return ValueTask.FromResult<object?>("ok");
        }

        var filter = new ValidationFilter<DummyDto>();

        var result = await filter.InvokeAsync(ctx, Next);

        called.Should().BeTrue();
        result.Should().Be("ok");
    }
}

internal sealed class FakeEndpointFilterInvocationContext : EndpointFilterInvocationContext
{
    private readonly HttpContext _http;
    private readonly object?[] _args;

    public FakeEndpointFilterInvocationContext(HttpContext http, object?[] args)
    {
        _http = http;
        _args = args;
    }

    public override HttpContext HttpContext => _http;

    public override object?[] Arguments => _args;

    public override T GetArgument<T>(int index) => (T)_args[index]!;
}