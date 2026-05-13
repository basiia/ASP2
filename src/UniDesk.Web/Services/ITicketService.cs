using UniDesk.Web.DTOs;
using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
    public interface ITicketService
    {
        PagedResult<TicketListDto> GetAll(TicketQueryParameters queryParams);
        Ticket? GetById(int id);
        List<Ticket> Search(string search);

        void Add(Ticket ticket);
        void Update(Ticket ticket);

        TicketReadDto Create(CreateTicketRequest request);
        TicketReadDto Update(int id, UpdateTicketRequest request);
        bool Delete(int id);
    }
}