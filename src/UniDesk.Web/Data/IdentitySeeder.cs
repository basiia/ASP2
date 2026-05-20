using Microsoft.AspNetCore.Identity;
using UniDesk.Web.Models;

namespace UniDesk.Web.Data
{
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";
        public const string AdminEmail = "admin@unidesk.local";
        public const string AdminPassword = "Admin123!";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(AdminRole));

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Nie udało się utworzyć roli Admin: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            var admin = await userManager.FindByEmailAsync(AdminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    OrganizationName = "UniDesk Administration"
                };

                var userResult = await userManager.CreateAsync(admin, AdminPassword);

                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Nie udało się utworzyć użytkownika Admin: " +
                        string.Join(", ", userResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                var addRoleResult = await userManager.AddToRoleAsync(admin, AdminRole);

                if (!addRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Nie udało się przypisać roli Admin: " +
                        string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
