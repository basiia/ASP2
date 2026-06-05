namespace UniDesk.Web.Data
{
    public class SeedDataOptions
    {
        public string AdminEmail { get; set; } = IdentitySeeder.AdminEmail;

        public string AdminPassword { get; set; } = IdentitySeeder.AdminPassword;

        public string DomainUserEmail { get; set; } = IdentitySeeder.DomainUserEmail;

        public string DomainUserPassword { get; set; } = IdentitySeeder.DomainUserPassword;

        public bool CreateDemoTickets { get; set; } = true;
    }
}
