
using NLog;

namespace CourseRegistration.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(
            RequestDelegate next,
            ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = Guid.NewGuid().ToString();

            context.TraceIdentifier = correlationId;
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            using (ScopeContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation(
                    "Incoming request: {Method} {Path}. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    correlationId);

                await _next(context);
            }
        }
    }
}
