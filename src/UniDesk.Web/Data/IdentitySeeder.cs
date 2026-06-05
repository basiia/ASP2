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

        public const string OutsiderEmail = "outsider@unidesk.local";
        public const string OutsiderPassword = "Outsider123!";

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

            var outsider = await EnsureUserAsync(
                userManager,
                options.OutsiderEmail,
                options.OutsiderPassword,
                "Outside Company");

            await EnsureClaimAsync(userManager, outsider, "EmployeeId", "EMP-OUT-001");

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
                    "Could not create role: " +
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
                    "Could not create user: " +
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
                    "Could not add role: " +
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
                    "Could not add claim: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private static async Task EnsureDemoTicketsAsync(
            UniDeskDbContext context,
            ApplicationUser admin,
            ApplicationUser domainUser)
        {
            var tickets = await context.Tickets
                .OrderBy(t => t.Id)
                .Take(2)
                .ToListAsync();

            if (tickets.Count == 0)
            {
                tickets.Add(new Ticket
                {
                    Title = "Problem z logowaniem",
                    Description = "Student nie moze zalogowac sie do systemu.",
                    Status = TicketStatus.New,
                    OwnerId = domainUser.Id,
                    OwnerName = domainUser.Email
                });

                tickets.Add(new Ticket
                {
                    Title = "Brak dostepu do materialow",
                    Description = "Materialy z kursu nie sa widoczne po wejscu na strone.",
                    Status = TicketStatus.InProgress,
                    OwnerId = domainUser.Id,
                    OwnerName = domainUser.Email
                });

                context.Tickets.AddRange(tickets);
                await context.SaveChangesAsync();
            }

            await EnsureTicketOwnersAsync(context, domainUser);

            if (await context.TicketComments.AnyAsync())
            {
                await EnsureStretchDemoCommentAsync(context, admin, tickets[0]);
                return;
            }

            var firstTicket = tickets[0];
            var secondTicket = tickets.Count > 1
                ? tickets[1]
                : new Ticket
                {
                    Title = "Brak dostepu do materialow",
                    Description = "Materialy z kursu nie sa widoczne po wejscu na strone.",
                    Status = TicketStatus.InProgress,
                    OwnerId = domainUser.Id,
                    OwnerName = domainUser.Email
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
                    Message = "**Dzien dobry**, problem pojawia sie od rana.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-20)
                },
                new TicketComment
                {
                    TicketId = firstTicket.Id,
                    AuthorId = admin.Id,
                    AuthorName = admin.Email ?? AdminEmail,
                    Message = "Sprawdzam konto. Kod bledu: `LOGIN-401`.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                },
                new TicketComment
                {
                    TicketId = secondTicket.Id,
                    AuthorId = domainUser.Id,
                    AuthorName = domainUser.Email ?? DomainUserEmail,
                    Message = "Test XSS: <script>alert('xss')</script>",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                });

            await context.SaveChangesAsync();
        }

        private static async Task EnsureStretchDemoCommentAsync(
            UniDeskDbContext context,
            ApplicationUser admin,
            Ticket ticket)
        {
            var exists = await context.TicketComments
                .AnyAsync(c => c.Message.Contains("Stretch demo"));

            if (exists)
            {
                return;
            }

            context.TicketComments.Add(new TicketComment
            {
                TicketId = ticket.Id,
                AuthorId = admin.Id,
                AuthorName = admin.Email ?? AdminEmail,
                Message = "Stretch demo: **bold text**, `code`, <script>alert('xss')</script>",
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        private static async Task EnsureTicketOwnersAsync(
            UniDeskDbContext context,
            ApplicationUser domainUser)
        {
            var ticketsWithoutOwner = await context.Tickets
                .Where(t => t.OwnerId == null)
                .ToListAsync();

            if (ticketsWithoutOwner.Count == 0)
            {
                return;
            }

            foreach (var ticket in ticketsWithoutOwner)
            {
                ticket.OwnerId = domainUser.Id;
                ticket.OwnerName = domainUser.Email;
            }

            await context.SaveChangesAsync();
        }
    }
}
