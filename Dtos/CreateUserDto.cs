using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos;

/* DTO (Data Transfer Object) som används när en ny användare skapas.
   Den separerar API:ts indata från databasen och gör det möjligt att validera
   inkommande data innan den sparas i databasen. */
public class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("Admin|User", ErrorMessage = "Role must be Admin or User.")]
    public string Role { get; set; } = "User";
}