namespace UniDesk.Web.DTOs
{
    public class TicketCommentDto
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string AuthorName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string MessageHtml { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
