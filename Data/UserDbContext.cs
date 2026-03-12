using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

/* Databaskontext för UserService.
   Den används av Entity Framework för att kommunicera med databasen. */
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    // Representerar tabellen "Users" i databasen
    public DbSet<User> Users { get; set; }
}