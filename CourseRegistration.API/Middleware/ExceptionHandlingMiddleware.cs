
using Microsoft.EntityFrameworkCore;
using NLog;

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
                    ArgumentException => StatusCodes.Status400BadRequest,
                    DbUpdateConcurrencyException =>
                        StatusCodes.Status409Conflict,
                    InvalidOperationException =>
                        StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                };
                if (statusCode == StatusCodes.Status409Conflict)
                {
                    _logger.LogWarning(ex,
                        "Conflict occurred. CorrelationId: {CorrelationId}",
                        context.TraceIdentifier);
                }
                else if (statusCode == StatusCodes.Status500InternalServerError)
                {
                    _logger.LogError(ex,
                        "Unexpected error. CorrelationId: {CorrelationId}",
                        context.TraceIdentifier);
                }
                else
                {
                    _logger.LogWarning(ex,
                        "Request rejected. CorrelationId: {CorrelationId}",
                        context.TraceIdentifier);
                }

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(new
                {
                    StatusCode = statusCode,
                    Message = statusCode == 500
                        ? "An unexpected error occurred."
                        : ex.Message,
                    CorrelationId = context.TraceIdentifier
                });
            }
        }
    }
}
