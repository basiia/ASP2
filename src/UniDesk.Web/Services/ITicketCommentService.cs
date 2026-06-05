using UniDesk.Web.DTOs;

namespace UniDesk.Web.Services
{
    public interface ITicketCommentService
    {
        Task<List<TicketCommentDto>> GetForTicketAsync(int ticketId);

        Task<TicketCommentDto> CreateAsync(
            int ticketId,
            string authorId,
            string authorName,
            CreateTicketCommentRequest request);
    }
}
