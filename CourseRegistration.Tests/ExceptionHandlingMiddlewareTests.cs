using System.Text.Json;
using CourseRegistration.API.Middleware;
using CourseRegistration.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CourseRegistration.Tests;

public class ExceptionHandlingMiddlewareTests
{
    public static TheoryData<Exception, int, LogLevel, string> ExceptionResponses => new()
    {
        {
            new DbUpdateConcurrencyException("Synthetic provider detail"), 409, LogLevel.Warning,
            "The course changed while your request was being processed. Refresh the page and try again."
        },
        { new InvalidOperationException("Course is full."), 409, LogLevel.Warning, "Course is full." },
        { new ArgumentException("Category does not exist."), 400, LogLevel.Error, "Category does not exist." },
        { new ResourceNotFoundException("Course not found."), 404, LogLevel.Error, "Course not found." },
        { new Exception("Synthetic internal detail"), 500, LogLevel.Error, "An unexpected error occurred." }
    };

    [Theory]
    [MemberData(nameof(ExceptionResponses))]
    public async Task InvokeAsync_ReturnsExpectedJsonAndLogsOnlySafeMetadata(
        Exception exception, int expectedStatus, LogLevel expectedLevel, string expectedMessage)
    {
        const string correlationId = "test-correlation-id";
        var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext { TraceIdentifier = correlationId };
        await using var body = new MemoryStream();
        context.Response.Body = body;
        var middleware = new ExceptionHandlingMiddleware(_ => Task.FromException(exception), logger.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(expectedStatus, context.Response.StatusCode);
        body.Position = 0;
        using var response = await JsonDocument.ParseAsync(body);
        var json = response.RootElement;
        Assert.Equal(3, json.EnumerateObject().Count());
        Assert.Equal(expectedStatus, json.GetProperty("statusCode").GetInt32());
        Assert.Equal(expectedMessage, json.GetProperty("message").GetString());
        Assert.Equal(correlationId, json.GetProperty("correlationId").GetString());

        var log = Assert.Single(logger.Invocations);
        Assert.Equal(nameof(ILogger.Log), log.Method.Name);
        Assert.Equal(expectedLevel, Assert.IsType<LogLevel>(log.Arguments[0]));
        Assert.Null(log.Arguments[3]);
        var state = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<string, object?>>>(log.Arguments[2]);
        var fields = state.ToDictionary(field => field.Key, field => field.Value);
        Assert.Equal(4, fields.Count);
        Assert.Equal(expectedStatus, Assert.IsType<int>(fields["StatusCode"]));
        Assert.Equal(exception.GetType().Name, fields["ExceptionType"]);
        Assert.Equal(correlationId, fields["CorrelationId"]);
        Assert.DoesNotContain(exception.Message, state.ToString()!);
    }
}
