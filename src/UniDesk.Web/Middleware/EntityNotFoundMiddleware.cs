using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.Exceptions;

namespace UniDesk.Web.Middleware
{
    public class EntityNotFoundMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EntityNotFoundMiddleware> _logger;

        public EntityNotFoundMiddleware(
            RequestDelegate next,
            ILogger<EntityNotFoundMiddleware> logger)
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
            catch (EntityNotFoundException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Entity not found during request {Path}",
                    context.Request.Path.Value);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Nie znaleziono zasobu",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problem);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception during request {Path}",
                    context.Request.Path.Value);

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Wystąpił błąd serwera",
                    Detail = "Wystąpił nieoczekiwany błąd aplikacji.",
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}