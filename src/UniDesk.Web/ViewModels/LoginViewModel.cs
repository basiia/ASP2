using System.ComponentModel.DataAnnotations;

namespace UniDesk.Web.ViewModels;
public class LoginViewModel
{
    [Required(ErrorMessage = "E-mail jest wymagany")]
    [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hasło jest wymagane")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

