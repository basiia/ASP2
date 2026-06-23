using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using UniDesk.Web.Services;

namespace UniDesk.Web.Models;
public enum TicketStatus
{
	New,
	InProgress,
	Closed
}

public class Ticket
{
	private readonly ISystemClock? _systemClock;

	[SetsRequiredMembers]
	public Ticket()
	{
		Title = string.Empty;
		Description = string.Empty;
		CreatedAt = DateTime.UtcNow;
	}

	[SetsRequiredMembers]
	public Ticket(ISystemClock systemClock)
	{
		_systemClock = systemClock;
		Title = string.Empty;
		Description = string.Empty;
		CreatedAt = _systemClock?.UtcNow ?? DateTime.UtcNow;
	}

	public int Id { get; set; }

	[Required(ErrorMessage = "TytuЕ‚ jest wymagany")]
	[StringLength(100)]
	public required string Title { get; set; }

	[Required(ErrorMessage = "Opis jest wymagany")]
	[StringLength(500)]
	public required string Description { get; set; }

	public TicketStatus Status { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	public string? OwnerId { get; set; }

	[StringLength(256)]
	public string? OwnerName { get; set; }

	public ApplicationUser? Owner { get; set; }

	public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}

