using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.DTOs
{
    public class CreateTicketCommentRequest : IValidatableObject
    {
        public const int MinMessageLength = 3;
        public const int MaxMessageLength = 1000;

        [Required(ErrorMessage = "Tresc komentarza jest wymagana")]
        [MinLength(MinMessageLength, ErrorMessage = "Komentarz musi miec co najmniej 3 znaki")]
        [StringLength(MaxMessageLength, ErrorMessage = "Komentarz nie moze byc dluzszy niz 1000 znakow")]
        public string Message { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                yield return new ValidationResult(
                    "Komentarz nie moze byc pusty",
                    new[] { nameof(Message) });

                yield break;
            }

            if (Message.Trim().Length < MinMessageLength)
            {
                yield return new ValidationResult(
                    "Komentarz musi miec co najmniej 3 znaki",
                    new[] { nameof(Message) });
            }
        }
    }
}
