namespace UniDesk.Web.DTOs;

public class TicketQueryParameters
{
	public string? Status { get; set; }
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 5;
	public bool Desc { get; set; } = false;
	public string? SortBy { get; set; }
}