namespace UniDesk.Web.Data;
public class SeedDataOptions
{
    public string AdminEmail { get; set; } = IdentitySeeder.AdminEmail;

    public string AdminPassword { get; set; } = IdentitySeeder.AdminPassword;

    public string DomainUserEmail { get; set; } = IdentitySeeder.DomainUserEmail;

    public string DomainUserPassword { get; set; } = IdentitySeeder.DomainUserPassword;

    public string OutsiderEmail { get; set; } = IdentitySeeder.OutsiderEmail;

    public string OutsiderPassword { get; set; } = IdentitySeeder.OutsiderPassword;

    public bool CreateDemoTickets { get; set; } = true;
}

