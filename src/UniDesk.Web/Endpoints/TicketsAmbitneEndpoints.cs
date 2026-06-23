using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using UniDesk.Web.DTOs;
using UniDesk.Web.Models;
using UniDesk.Web.Services;

namespace UniDesk.Web.Endpoints;
public static class TicketsAmbitneEndpoints
{
    public static IEndpointRouteBuilder MapAmbitneTicketsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ambitne/login", LoginForToken)
            .AllowAnonymous()
            .WithTags("LAB 11 AMBITNE");

        var group = app.MapGroup("/api/ambitne/tickets")
            .WithTags("LAB 11 AMBITNE - Tickets")
            .RequireAuthorization("BearerUser");

        group.MapGet("/", GetTickets);

        group.MapGet("/me", GetCurrentUser);

        group.MapGet("/domain-resource", GetDomainResource)
            .RequireAuthorization("BearerTopUni");

        group.MapDelete("/{id:int}", DeleteTicket)
            .RequireAuthorization("BearerAdmin");

        return app;
    }

    private static async Task<IResult> LoginForToken(
        LoginTokenRequest request,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Results.Unauthorized();
        }

        var passwordIsValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordIsValid)
        {
            return Results.Unauthorized();
        }

        var principal = await signInManager.CreateUserPrincipalAsync(user);

        return Results.SignIn(
            principal,
            authenticationScheme: IdentityConstants.BearerScheme);
    }

    private static IResult GetTickets(ITicketService ticketService)
    {
        var result = ticketService.GetAll(new TicketQueryParameters());
        return Results.Ok(result);
    }


    private static IResult GetCurrentUser(ClaimsPrincipal user)
    {
        var data = new
        {
            name = user.Identity?.Name,
            email = user.FindFirstValue(ClaimTypes.Email),
            employeeId = user.FindFirstValue("EmployeeId"),
            roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
        };

        return Results.Ok(data);
    }

    private static IResult GetDomainResource(ClaimsPrincipal user)
    {
        var data = new
        {
            message = "Dostęp przyznany dla domeny @top-uni.edu.pl",
            email = user.FindFirstValue(ClaimTypes.Email),
            employeeId = user.FindFirstValue("EmployeeId")
        };

        return Results.Ok(data);
    }

    private static IResult DeleteTicket(int id, ITicketService ticketService)
    {
        var ticket = ticketService.GetById(id);

        if (ticket == null)
        {
            return Results.NotFound();
        }

        ticketService.Delete(id);

        return Results.NoContent();
    }
}

public record LoginTokenRequest(string Email, string Password);


