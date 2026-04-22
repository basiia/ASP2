using UniDesk.Web.Services;
using Microsoft.EntityFrameworkCore;
using UniDesk.Web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITicketService, DbTicketService>();
builder.Services.AddScoped<ITicketRepository, DbTicketRepository>();
builder.Services.AddScoped<ISystemClock, SystemClock>();

builder.Services.AddDbContext<UniDeskDbContext>(options =>
	options.UseInMemoryDatabase("TestDb"));

var app = builder.Build();

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

app.Run();

public partial class Program { }