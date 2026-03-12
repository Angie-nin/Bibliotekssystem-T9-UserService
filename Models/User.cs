namespace UserService.Models;

/* Representerar en användare i systemet.
   Denna modell används av Entity Framework för att lagra användare i databasen. */
public class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Hashat lösenord. Själva lösenordet sparas aldrig i databasen.
    public string PasswordHash { get; set; } = string.Empty;

    // Användarens roll i systemet (t.ex. Admin eller User)
    public string Role { get; set; } = "User";
}