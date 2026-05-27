using Serilog.Context;

namespace UniDesk.Web.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";

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
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var headerValue)
                                && !string.IsNullOrWhiteSpace(headerValue.ToString())
                ? headerValue.ToString()
                : Guid.NewGuid().ToString("N");

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation(
                    "Request received with CorrelationId {CorrelationId} for path {Path}",
                    correlationId,
                    context.Request.Path.Value);

                await _next(context);
            }
        }
    }
}