using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniDesk.Web.Models; 

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
	public void Configure(EntityTypeBuilder<Ticket> builder)
	{
		builder.Property(t => t.Title)
			.IsRequired()  
			.HasMaxLength(100);  

		builder.Property(t => t.Description)
			.IsRequired()  
			.HasMaxLength(500);  

		builder.Property(t => t.Status)
			.IsRequired();  

		builder.Property(t => t.CreatedAt)
			.IsRequired();  

		builder.Property(t => t.UpdatedAt)
			.IsRequired();  

		builder.Property(t => t.OwnerName)
			.HasMaxLength(256);

		builder.HasOne(t => t.Owner)
			.WithMany()
			.HasForeignKey(t => t.OwnerId)
			.OnDelete(DeleteBehavior.SetNull);
	}
}
