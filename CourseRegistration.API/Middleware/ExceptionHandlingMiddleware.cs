
using CourseRegistration.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using ScopeContext = NLog.ScopeContext;

namespace CourseRegistration.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                using var correlationScope =
                   ScopeContext.PushProperty("CorrelationId", context.TraceIdentifier);
                int statusCode = ex switch
                {
                    ResourceNotFoundException => StatusCodes.Status404NotFound,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    DbUpdateConcurrencyException =>
                        StatusCodes.Status409Conflict,
                    InvalidOperationException =>
                        StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                };
                var logLevel = statusCode == StatusCodes.Status409Conflict
                    ? LogLevel.Warning
                    : LogLevel.Error;

                _logger.Log(logLevel,
                    "Request failed with HTTP {StatusCode} ({ExceptionType}). CorrelationId: {CorrelationId}",
                    statusCode, ex.GetType().Name, context.TraceIdentifier);

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = statusCode,
                    Message = ex switch
                    {
                        DbUpdateConcurrencyException =>
                            "The course changed while your request was being processed. Refresh the page and try again.",
                        _ when statusCode == StatusCodes.Status500InternalServerError =>
                            "An unexpected error occurred.",
                        _ => ex.Message
                    },
                    CorrelationId = context.TraceIdentifier
                });
            }
        }
    }
}
