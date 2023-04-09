using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace OnionCSharpPocTest.Infrastructure.Middlewares;

public class UnitTestExceptionMiddleware
{
    private readonly Mock<ILogger<ExceptionMiddleware>> _mockLogger = new();
    private readonly ExceptionMiddleware _middleware;

    public UnitTestExceptionMiddleware()
    {
        _middleware = new ExceptionMiddleware(_mockLogger.Object);
    }
    
    [Fact]
    public async Task InvokeAsync_Should_CallNext_When_NoExceptionThrown()
    {
        // Arrange
        var mockContext = new Mock<HttpContext>();
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await _middleware.InvokeAsync(mockContext.Object, next);

        // Assert
        nextCalled.Should().BeTrue();
    }
}