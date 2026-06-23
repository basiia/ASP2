using System.ComponentModel.DataAnnotations;
using UniDesk.Web.Models;

namespace UniDesk.Web.ViewModels;
public class TicketEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tytul jest wymagany")]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis jest wymagany")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; }
}

