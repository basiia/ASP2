using Microsoft.EntityFrameworkCore;
using UniDesk.Web.Models;

public class UniDeskDbContext : DbContext
{
	public UniDeskDbContext(DbContextOptions<UniDeskDbContext> options) : base(options) { }

	public DbSet<Ticket> Tickets { get; set; }

	public override int SaveChanges()
	{
		var entries = ChangeTracker.Entries()
			.Where(e => e.Entity is Ticket && (e.State == EntityState.Added || e.State == EntityState.Modified));

		foreach (var entry in entries)
		{
			if (entry.State == EntityState.Added)
			{
				((Ticket)entry.Entity).CreatedAt = DateTime.Now;
			}
			((Ticket)entry.Entity).UpdatedAt = DateTime.Now;
		}

		return base.SaveChanges();
	}
}