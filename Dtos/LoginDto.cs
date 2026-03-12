using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos;

/* DTO som används vid inloggning.
   Den innehåller endast den information som behövs för att verifiera en användare. */
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}