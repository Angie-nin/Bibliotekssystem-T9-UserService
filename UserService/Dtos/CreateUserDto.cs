using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos;

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