using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Presentation.Exceptions;
using System.Text.Json;
using Xunit;

namespace SkillFlow.Tests.Presentation;

public class GlobalExceptionHandlerTests
{
    private static GlobalExceptionHandler CreateHandler()
    {
        var logger = new Mock<ILogger<GlobalExceptionHandler>>();
        return new GlobalExceptionHandler(logger.Object);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    [Fact]
    public async Task Should_return_404_for_AttendeeNotFoundException()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        var ex = new AttendeeNotFoundException(new AttendeeId(Guid.NewGuid()));

        var handled = await handler.TryHandleAsync(context, ex, default);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Should_return_409_for_EmailAlreadyExistsException()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        var ex = new EmailAlreadyExistsException(
            SkillFlow.Domain.Entities.Attendees.Email.Create("test@gmail.com"));

        var handled = await handler.TryHandleAsync(context, ex, default);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task Should_return_400_for_InvalidEmailException()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        var ex = new InvalidEmailException("invalid");

        var handled = await handler.TryHandleAsync(context, ex, default);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Should_return_500_for_unknown_exception()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        var ex = new Exception("boom");

        var handled = await handler.TryHandleAsync(context, ex, default);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Should_write_problem_details_body()
    {
        var handler = CreateHandler();
        var context = CreateContext();

        var ex = new Exception("boom");

        await handler.TryHandleAsync(context, ex, default);

        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var json = await new StreamReader(context.Response.Body).ReadToEndAsync();

        json.Should().Contain("Internal Server Error");
        json.Should().Contain("boom");
    }
}