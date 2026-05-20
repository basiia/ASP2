using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
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

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await EnsureRoleAsync(roleManager, AdminRole);

            var admin = await EnsureUserAsync(
                userManager,
                AdminEmail,
                AdminPassword,
                "UniDesk Administration");

            await EnsureUserRoleAsync(userManager, admin, AdminRole);
            await EnsureClaimAsync(userManager, admin, "EmployeeId", "EMP-ADMIN-001");

            var domainUser = await EnsureUserAsync(
                userManager,
                DomainUserEmail,
                DomainUserPassword,
                "Top Uni");

            await EnsureClaimAsync(userManager, domainUser, "EmployeeId", "EMP-TOP-001");
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
    }
}

