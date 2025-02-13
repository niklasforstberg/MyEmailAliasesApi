using EmailAliasApi.Data;
using EmailAliasApi.Models;
using EmailAliasApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailAliasApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, EmailAliasDbContext db) =>
        {
            if (await db.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Results.BadRequest("Email already registered");
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Username = request.Email, // Using email as username by default
                Role = User.UserRole.USER
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok("User registered successfully");
        });

        group.MapPost("/login", async (LoginRequest request, EmailAliasDbContext db, IConfiguration config) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.BadRequest("Invalid credentials");
            }

            var token = GenerateJwtToken(user, config);
            return Results.Ok(new { Token = token });
        });

        // Development-only endpoint for creating admin user
        group.MapPost("/create-admin", async (RegisterRequest request, EmailAliasDbContext db, IWebHostEnvironment env) =>
        {
            if (!env.IsDevelopment())
            {
                return Results.Forbid();
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Username = request.Email,
                Role = User.UserRole.ADMIN
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok("Admin user created successfully");
        });

        // Development-only endpoint for generating long-lived tokens
        group.MapPost("/dev-token", async (LoginRequest request, EmailAliasDbContext db, IConfiguration config, IWebHostEnvironment env) =>
        {
            if (!env.IsDevelopment())
            {
                return Results.Forbid();
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.BadRequest("Invalid credentials");
            }

            var token = GenerateDevJwtToken(user, config);
            return Results.Ok(new { Token = $"Bearer {token}" });
        });

        group.MapGet("/me", async (HttpContext context, EmailAliasDbContext db, ILogger<Program> logger) =>
        {
            var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            logger.LogInformation("User ID from claim: {UserId}", userIdString);

            if (userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                logger.LogWarning("Failed to parse user ID: {UserId}", userIdString);
                return Results.Unauthorized();
            }

            logger.LogInformation("Parsed User ID: {UserId}", userId);

            try 
            {
                var user = await db.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Username = u.Username,
                        Role = u.Role.ToString()
                    })
                    .FirstOrDefaultAsync();
                
                logger.LogInformation("Retrieved user: {User}", user);

                if (user == null)
                {
                    logger.LogWarning("User not found for ID: {UserId}", userId);
                    return Results.NotFound();
                }

                return Results.Ok(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
                throw;
            }
        }).RequireAuthorization();
    }

    private static string GenerateDevJwtToken(User user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddYears(1), // 1 year expiration for development
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateJwtToken(User user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 