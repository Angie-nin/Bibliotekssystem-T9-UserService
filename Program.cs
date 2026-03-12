using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Security;

var builder = WebApplication.CreateBuilder(args);

// Registrerar tjänster som används i applikationen
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

var app = builder.Build();

// Swagger används endast i utvecklingsmiljö för att testa och dokumentera API:t
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware som skyddar skrivande endpoints med API-nyckel
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();