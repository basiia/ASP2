using UniDesk.Web.DTOs;
using UniDesk.Web.Filters;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

namespace UniDesk.Web.Endpoints
{
    public static class TicketsV2Endpoints
    {
        public static IEndpointRouteBuilder MapTicketsV2Endpoints(this IEndpointRouteBuilder app)
        {
            var ticketsV2 = app.MapGroup("/api/v2/tickets")
                .WithTags("Tickets v2")
                .AddEndpointFilter<RequestTimingFilter>()
                .AddEndpointFilter<ValidationFilter>();

            ticketsV2.MapGet("", (ITicketService ticketService) =>
            {
                var result = ticketService.GetAll(new TicketQueryParameters
                {
                    Page = 1,
                    PageSize = 100
                });

                return Results.Ok(result);
            })
            .WithName("GetTicketsV2")
            .WithOpenApi();

            ticketsV2.MapPost("", (CreateTicketRequest request, ITicketService ticketService) =>
            {
                var created = ticketService.Create(request);

                return Results.Created($"/api/v2/tickets/{created.Id}", created);
            })
            .WithName("CreateTicketV2")
            .WithOpenApi();

            ticketsV2.MapPut("/{id:int}", (int id, UpdateTicketRequest request, ITicketService ticketService) =>
            {
                var updated = ticketService.Update(id, request);

                return Results.Ok(updated);
            })
            .WithName("UpdateTicketV2")
            .WithOpenApi();

            ticketsV2.MapDelete("/{id:int}", (int id, ITicketService ticketService) =>
            {
                ticketService.Delete(id);

                return Results.NoContent();
            })
            .WithName("DeleteTicketV2")
            .WithOpenApi();

            return app;
        }
    }
}