using System.Diagnostics;

namespace UniDesk.Web.Filters
{
    public class RequestTimingFilter : IEndpointFilter
    {
        private readonly ILogger<RequestTimingFilter> _logger;

        public RequestTimingFilter(ILogger<RequestTimingFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;

            _logger.LogInformation("Start żądania Minimal API: {Method} {Path}", method, path);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                return await next(context);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                    "Koniec żądania Minimal API: {Method} {Path}. Czas: {ElapsedMilliseconds} ms",
                    method,
                    path,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}