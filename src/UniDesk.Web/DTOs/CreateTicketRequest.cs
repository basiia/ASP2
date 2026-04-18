using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.DTOs
{
	public class CreateTicketRequest
	{
		[Required(ErrorMessage = "Tytuł jest wymagany")]
		[MinLength(1, ErrorMessage = "Tytuł nie może być pusty")]
		[StringLength(100)]
		public string Title { get; set; } = string.Empty;

		[Required(ErrorMessage = "Opis jest wymagany")]
		[MinLength(1, ErrorMessage = "Opis nie może być pusty")]
		[StringLength(500)]
		public string Description { get; set; } = string.Empty;
	}
}