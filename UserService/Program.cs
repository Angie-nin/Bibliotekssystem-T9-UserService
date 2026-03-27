using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UserService.Data;
using UserService.Models;
using UserService.Security;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Data Source=users.db";

// Om appen kör i Azure App Service på Windows
var home = Environment.GetEnvironmentVariable("HOME");
if (!string.IsNullOrWhiteSpace(home))
{
    var dataFolder = Path.Combine(home, "site", "data");
    Directory.CreateDirectory(dataFolder);

    var dbPath = Path.Combine(dataFolder, "users.db");
    connectionString = $"Data Source={dbPath}";
}

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// OpenAPI + API docs i utvecklingsmiljö
app.UseSwagger();
app.MapScalarApiReference(options =>
{
    options.Title = "UserService API";
    options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
});

app.UseHttpsRedirection();

// Skyddar skrivande endpoints med API-nyckel
app.UseMiddleware<ApiKeyMiddleware>();

app.MapGet("/", () => "UserService API is running. Go to /scalar for documentation.");

app.UseCors("ReactApp");

app.MapControllers();

// Ser till att databasen och tabellerna skapas/uppdateras + seedar testdata
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Users.Any())
    {
        var passwordHasher = new PasswordHasher<User>();

        var users = new List<User>
        {
            new User
            {
                FullName = "Anna Andersson",
                Email = "anna.andersson@test.se",
                Role = "User"
            },
            new User
            {
                FullName = "Erik Eriksson",
                Email = "erik.eriksson@test.se",
                Role = "Admin"
            },
            new User
            {
                FullName = "Sara Svensson",
                Email = "sara.svensson@test.se",
                Role = "User"
            },
            new User
            {
                FullName = "Johan Johansson",
                Email = "johan.johansson@test.se",
                Role = "User"
            },
            new User
            {
                FullName = "Maria Karlsson",
                Email = "maria.karlsson@test.se",
                Role = "Admin"
            }
        };

        foreach (var user in users)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, "Test123!");
        }

        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();
    }
}

app.Run();