using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UniDesk.Web.Models;

namespace UniDesk.Web.Authorization
{
    public class TicketAccessHandler : AuthorizationHandler<TicketAccessRequirement, Ticket>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TicketAccessRequirement requirement,
            Ticket resource)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && resource.OwnerId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
