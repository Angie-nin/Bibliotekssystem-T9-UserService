using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UserService.Data;
using UserService.Security;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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

var app = builder.Build();

app.UseSwagger();
app.MapScalarApiReference(options =>
{
    options.Title = "UserService API";
    options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
});

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapGet("/", () => "UserService API is running. Go to /scalar for documentation.");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();