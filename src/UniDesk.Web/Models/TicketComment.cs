using System.ComponentModel.DataAnnotations;
using UniDesk.Web.DTOs;

namespace UniDesk.Web.Models;
public class TicketComment
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public Ticket? Ticket { get; set; }

    [Required]
    public string AuthorId { get; set; } = string.Empty;

    public ApplicationUser? Author { get; set; }

    [Required]
    [StringLength(256)]
    public string AuthorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tresc komentarza jest wymagana")]
    [MinLength(CreateTicketCommentRequest.MinMessageLength)]
    [StringLength(CreateTicketCommentRequest.MaxMessageLength, ErrorMessage = "Komentarz nie moze byc dluzszy niz 1000 znakow")]
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

