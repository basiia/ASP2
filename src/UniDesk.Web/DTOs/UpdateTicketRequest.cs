using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.DTOs
{
    public class UpdateTicketRequest
    {
        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [MinLength(1, ErrorMessage = "Tytuł nie może być pusty")]
        [StringLength(100, ErrorMessage = "Tytuł nie może być dłuższy niż 100 znaków")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis jest wymagany")]
        [MinLength(1, ErrorMessage = "Opis nie może być pusty")]
        [StringLength(500, ErrorMessage = "Opis nie może być dłuższy niż 500 znaków")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 2, ErrorMessage = "Status musi mieć wartość od 0 do 2")]
        public int Status { get; set; }
    }
}