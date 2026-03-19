using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UsersController(UserDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/Users/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            })
            .FirstOrDefaultAsync();

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
    {
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Role = dto.Role
        };

        // Hasha EFTER att objektet skapats
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, result);
    }

    // PUT: api/Users/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UserDto dto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
            return NotFound();

        var isDemotingLastAdmin = user.Role == "Admin" &&
                                  dto.Role == "User" &&
                                  await IsLastAdminAsync(id);

        if (isDemotingLastAdmin)
            return BadRequest("Cannot change the last admin user to User.");

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.Role = dto.Role;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
            return NotFound();

        if (await IsLastAdminAsync(id))
            return BadRequest("Cannot delete the last admin user.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return Unauthorized("Invalid email or password");

        var result = _passwordHasher.VerifyHashedPassword(default!, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password");

        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };

        return Ok(userDto);
    }

    [HttpGet("role/{role}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return BadRequest("Role is required.");

        role = role.Trim();

        if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
            !role.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Role must be Admin or User.");
        }

        var normalizedRole = char.ToUpper(role[0]) + role[1..].ToLower();

        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == normalizedRole)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();

        return Ok(users);
    }
    private async Task<bool> IsLastAdminAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null || user.Role != "Admin")
            return false;

        var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
        return adminCount == 1;
    }
}