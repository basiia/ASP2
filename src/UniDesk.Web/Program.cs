using Microsoft.EntityFrameworkCore;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

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

// LAB 10 CORE - Minimal API v2

app.MapGet("/api/v2/tickets", (ITicketService ticketService) =>
{
    var result = ticketService.GetAll(new TicketQueryParameters
    {
        Page = 1,
        PageSize = 100
    });

    return Results.Ok(result);
});

app.MapPost("/api/v2/tickets", (CreateTicketRequest request, ITicketService ticketService) =>
{
    var created = ticketService.Create(request);

    return Results.Created($"/api/v2/tickets/{created.Id}", created);
});

app.MapDelete("/api/v2/tickets/{id:int}", (int id, ITicketService ticketService) =>
{
    var deleted = ticketService.Delete(id);

    if (!deleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.Run();

public partial class Program { }