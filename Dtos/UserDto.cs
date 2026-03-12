using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos;

/* DTO som används när användardata skickas tillbaka från API:t.
   Den innehåller endast information som är säker att exponera till klienten.
   Exempelvis skickas inte PasswordHash med här. */
public class UserDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("Admin|User", ErrorMessage = "Role must be Admin or User.")]
    public string Role { get; set; } = "User";
}