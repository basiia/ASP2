using UniDesk.Web.DTOs;
using UniDesk.Web.Models;

public interface ITicketRepository
{
	Ticket? GetById(int id);
	void Add(Ticket ticket);
	void Update(Ticket ticket);
	void SaveChanges();
	IQueryable<Ticket> GetAll(TicketQueryParameters queryParams); 
	List<Ticket> Search(string search);
}