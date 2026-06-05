using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using UniDesk.Web.Models;

public class UniDeskDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public UniDeskDbContext(DbContextOptions<UniDeskDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<TicketComment> TicketComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TicketConfiguration());
        modelBuilder.ApplyConfiguration(new TicketCommentConfiguration());
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Ticket &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((Ticket)entry.Entity).CreatedAt = DateTime.UtcNow;
            }

            ((Ticket)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        SetCommentDates();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Ticket &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((Ticket)entry.Entity).CreatedAt = DateTime.UtcNow;
            }

            ((Ticket)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        SetCommentDates();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetCommentDates()
    {
        var comments = ChangeTracker.Entries()
            .Where(e => e.Entity is TicketComment && e.State == EntityState.Added);

        foreach (var entry in comments)
        {
            var comment = (TicketComment)entry.Entity;

            if (comment.CreatedAt == default)
            {
                comment.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}
