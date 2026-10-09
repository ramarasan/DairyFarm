using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DairyFarm.Api.Data;
using DairyFarm.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DairyFarm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, IConfiguration cfg) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username && u.IsActive);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid username or password" });

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
        var expires = DateTime.UtcNow.AddHours(double.Parse(cfg["Jwt:ExpiryHours"] ?? "8"));
        var token = new JwtSecurityToken(cfg["Jwt:Issuer"], cfg["Jwt:Audience"], claims,
            expires: expires, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            fullName = user.FullName,
            role = user.Role,
            expires
        });
    }

    // Admin creates user accounts
    [Authorize(Roles = "Admin")]
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(RegisterUserRequest r)
    {
        if (r.Role is not ("Admin" or "User")) return BadRequest(new { message = "Role must be Admin or User" });
        if (await db.Users.AnyAsync(u => u.Username == r.Username))
            return Conflict(new { message = "Username already exists" });

        db.Users.Add(new User
        {
            FullName = r.FullName, Username = r.Username, Role = r.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password), CreatedAt = DateTime.Now
        });
        await db.SaveChangesAsync();
        return Ok(new { message = "User created" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<IActionResult> Users() =>
        Ok(await db.Users.Select(u => new { u.UserId, u.FullName, u.Username, u.Role, u.IsActive }).ToListAsync());

    [Authorize(Roles = "Admin")]
    [HttpPut("users/{id}/active")]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool active)
    {
        var u = await db.Users.FindAsync(id);
        if (u is null) return NotFound();
        u.IsActive = active;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
