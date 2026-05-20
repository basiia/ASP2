using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "E-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Nazwa organizacji może mieć maksymalnie 100 znaków")]
        public string? OrganizationName { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć minimum 6 znaków")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Hasła nie są takie same")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}
