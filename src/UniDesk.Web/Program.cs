using Microsoft.EntityFrameworkCore;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using UniDesk.Web.Middleware;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITicketService, DbTicketService>();
builder.Services.AddScoped<ITicketRepository, DbTicketRepository>();
builder.Services.AddScoped<ISystemClock, SystemClock>();

builder.Services.AddDbContext<UniDeskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";

    await next();
});

app.UseMiddleware<EntityNotFoundMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();


// LAB 10 STRETCH - Minimal API v2 grouped endpoints

var ticketsV2 = app.MapGroup("/api/v2/tickets")
    .WithTags("Tickets v2");

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

app.Run();

public partial class Program { }