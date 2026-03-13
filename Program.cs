using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using UserService.Data;
using UserService.Security;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// OpenAPI + API docs i utvecklingsmiljö
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.MapScalarApiReference(options =>
    {
        options.Title = "UserService API";
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

app.UseHttpsRedirection();

// Skyddar skrivande endpoints med API-nyckel
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

// Ser till att databasen och tabellerna skapas/uppdateras
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    dbContext.Database.Migrate();
}

app.Run();