using UniDesk.Web.DTOs;

namespace UniDesk.Web.ViewModels
{
    public class TicketDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<TicketCommentDto> Comments { get; set; } = new();

        public CreateTicketCommentRequest NewComment { get; set; } = new();
    }
}
