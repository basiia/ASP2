using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniDesk.Web.Models;

namespace UniDesk.Web.Data
{
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";

        public const string AdminEmail = "admin@unidesk.local";
        public const string AdminPassword = "Admin123!";

        public const string DomainUserEmail = "employee@top-uni.edu.pl";
        public const string DomainUserPassword = "Employee123!";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<UniDeskDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<SeedDataOptions>>().Value;

            await context.Database.MigrateAsync();

            await EnsureRoleAsync(roleManager, AdminRole);

            var admin = await EnsureUserAsync(
                userManager,
                options.AdminEmail,
                options.AdminPassword,
                "UniDesk Administration");

            await EnsureUserRoleAsync(userManager, admin, AdminRole);
            await EnsureClaimAsync(userManager, admin, "EmployeeId", "EMP-ADMIN-001");

            var domainUser = await EnsureUserAsync(
                userManager,
                options.DomainUserEmail,
                options.DomainUserPassword,
                "Top Uni");

            await EnsureClaimAsync(userManager, domainUser, "EmployeeId", "EMP-TOP-001");

            if (options.CreateDemoTickets)
            {
                await EnsureDemoTicketsAsync(context, admin, domainUser);
            }
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Nie udało się utworzyć roli: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string organizationName)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                if (user.OrganizationName != organizationName)
                {
                    user.OrganizationName = organizationName;
                    await userManager.UpdateAsync(user);
                }

                return user;
            }

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                OrganizationName = organizationName
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Nie udało się utworzyć użytkownika: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user;
        }

        private static async Task EnsureUserRoleAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string roleName)
        {
            if (await userManager.IsInRoleAsync(user, roleName))
            {
                return;
            }

            var result = await userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Nie udało się przypisać roli: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private static async Task EnsureClaimAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string claimType,
            string claimValue)
        {
            var claims = await userManager.GetClaimsAsync(user);

            if (claims.Any(c => c.Type == claimType))
            {
                return;
            }

            var result = await userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Nie udało się dodać claimu: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private static async Task EnsureDemoTicketsAsync(
            UniDeskDbContext context,
            ApplicationUser admin,
            ApplicationUser domainUser)
        {
            if (await context.TicketComments.AnyAsync())
            {
                return;
            }

            var tickets = await context.Tickets
                .OrderBy(t => t.Id)
                .Take(2)
                .ToListAsync();

            if (tickets.Count == 0)
            {
                tickets.Add(new Ticket
                {
                    Title = "Problem z logowaniem",
                    Description = "Student nie może zalogować się do systemu.",
                    Status = TicketStatus.Open
                });

                tickets.Add(new Ticket
                {
                    Title = "Brak dostępu do materiałów",
                    Description = "Materiały z kursu nie są widoczne po wejściu na stronę.",
                    Status = TicketStatus.InProgress
                });

                context.Tickets.AddRange(tickets);
                await context.SaveChangesAsync();
            }

            var firstTicket = tickets[0];
            var secondTicket = tickets.Count > 1
                ? tickets[1]
                : new Ticket
                {
                    Title = "Brak dostępu do materiałów",
                    Description = "Materiały z kursu nie są widoczne po wejściu na stronę.",
                    Status = TicketStatus.InProgress
                };

            if (secondTicket.Id == 0)
            {
                context.Tickets.Add(secondTicket);
                await context.SaveChangesAsync();
            }

            context.TicketComments.AddRange(
                new TicketComment
                {
                    TicketId = firstTicket.Id,
                    AuthorId = domainUser.Id,
                    AuthorName = domainUser.Email ?? DomainUserEmail,
                    Message = "Dzień dobry, problem pojawia się od rana.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-20)
                },
                new TicketComment
                {
                    TicketId = firstTicket.Id,
                    AuthorId = admin.Id,
                    AuthorName = admin.Email ?? AdminEmail,
                    Message = "Sprawdzam konto i historię logowania.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                },
                new TicketComment
                {
                    TicketId = secondTicket.Id,
                    AuthorId = domainUser.Id,
                    AuthorName = domainUser.Email ?? DomainUserEmail,
                    Message = "Po odświeżeniu strony nadal nie widzę plików.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                });

            await context.SaveChangesAsync();
        }
    }
}
