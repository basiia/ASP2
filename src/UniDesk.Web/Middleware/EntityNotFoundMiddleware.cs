using Microsoft.AspNetCore.Mvc;
using UniDesk.Web.Exceptions;

namespace UniDesk.Web.Middleware
{
    public class EntityNotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public EntityNotFoundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (EntityNotFoundException ex)
            {
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
        }
    }
}