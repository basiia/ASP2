using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniDesk.Web.Models;
using UniDesk.Web.Services;
using UniDesk.Web.Middleware;
using UniDesk.Web.Endpoints;
using UniDesk.Web.Filters;
using UniDesk.Web.Data;
using UniDesk.Web.HealthChecks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Formatting.Json;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using UniDesk.Web.Authorization;

var builder = WebApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", environmentName)
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File(
        formatter: new JsonFormatter(),
        path: "Logs/unidesk-log-.json",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketRepository, DbTicketRepository>();
builder.Services.AddScoped<ITicketCommentService, DbTicketCommentService>();
builder.Services.AddSingleton<IMarkdownFormatter, SimpleMarkdownFormatter>();
builder.Services.AddSingleton<IAuthorizationHandler, TicketAccessHandler>();
builder.Services.AddScoped<ISystemClock, SystemClock>();

builder.Services.AddScoped<RequestTimingFilter>();
builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddDbContext<UniDeskDbContext>(options =>
    options
        .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        .LogTo(
            Console.WriteLine,
            new[] { DbLoggerCategory.Database.Command.Name },
            LogLevel.Information));

builder.Services.Configure<SeedDataOptions>(
    builder.Configuration.GetSection("SeedData"));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("comments", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.User.Identity?.Name
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddHealthChecks()
    .AddCheck(
        "application",
        () => HealthCheckResult.Healthy("Application is running"),
        tags: new[] { "live", "ready" })
    .AddDbContextCheck<UniDeskDbContext>(
        name: "database",
        tags: new[] { "ready" })
    .AddCheck<DiskSpaceHealthCheck>(
        name: "disk_space",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "ready" });

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<UniDeskDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanAccessTicket", policy =>
    {
        policy.Requirements.Add(new TicketAccessRequirement());
    });

    options.AddPolicy("BearerUser", policy =>
    {
        policy.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy("BearerAdmin", policy =>
    {
        policy.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });

    options.AddPolicy("BearerTopUni", policy =>
    {
        policy.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
        policy.RequireAuthenticatedUser();

        policy.RequireAssertion(context =>
        {
            var email = context.User.FindFirstValue(ClaimTypes.Email)
                        ?? context.User.Identity?.Name;

            return email != null &&
                   email.EndsWith("@top-uni.edu.pl", StringComparison.OrdinalIgnoreCase);
        });
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";

    await next();
});

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<EntityNotFoundMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
        {
            diagnosticContext.Set("CorrelationId", correlationId);
        }

        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

app.UseRouting();

app.UseAuthentication();
app.UseRateLimiter();
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

app.MapTicketsV2Endpoints();

app.MapAmbitneTicketsEndpoints();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = WriteHealthResponse
}).AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
}).AllowAnonymous();

await IdentitySeeder.SeedAsync(app.Services);

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var response = new
    {
        status = report.Status.ToString(),
        timestamp = DateTimeOffset.UtcNow,
        totalDuration = report.TotalDuration.ToString(),
        checks = report.Entries.ToDictionary(
            entry => entry.Key,
            entry => new
            {
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString(),
                exception = entry.Value.Exception?.Message,
                data = entry.Value.Data
            })
    };

    var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
}
public partial class Program { }
