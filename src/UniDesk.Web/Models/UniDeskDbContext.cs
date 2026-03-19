using Microsoft.EntityFrameworkCore;
using UniDesk.Web.Models;

public class UniDeskDbContext : DbContext
{
	public UniDeskDbContext(DbContextOptions<UniDeskDbContext> options) : base(options) { }

	public DbSet<Ticket> Tickets { get; set; } 
}