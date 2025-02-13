using EmailAliasApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EmailAliasApi.Models;

namespace EmailAliasApi.Endpoints;

public static class AliasEndpoints
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        app.MapGet("/api/aliases", GetAuthenticatedUserAliases)
            .WithName("GetAuthenticatedUserAliases")
            .WithOpenApi()
            .RequireAuthorization();

        app.MapGet("/api/users/{userId}/aliases", GetUserAliases)
            .WithName("GetUserAliases")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireClaim(ClaimTypes.Role, User.UserRole.ADMIN.ToString()));

        app.MapGet("/api/aliases/{aliasId}", GetAuthenticatedUserAlias)
            .WithName("GetAuthenticatedUserAlias")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAuthenticatedUserAliases(HttpContext context, EmailAliasDbContext db)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Results.Unauthorized();
        }

        var aliases = await db.EmailAliases
            .Include(a => a.ForwardingAddresses)
            .Where(a => a.UserId == userId)
            .Select(a => new
            {
                a.Id,
                Alias = a.AliasAddress,
                a.Status,
                a.CreatedAt,
                a.UserId,
                ForwardingAddresses = a.ForwardingAddresses.Select(f => new
                {
                    f.Id,
                    f.ForwardingAddress
                }).ToList()
            })
            .ToListAsync();
        return Results.Ok(aliases);
    }

    private static async Task<IResult> GetUserAliases(int userId, EmailAliasDbContext db)
    {
        var aliases = await db.EmailAliases
            .Include(a => a.ForwardingAddresses)
            .Where(a => a.UserId == userId)
            .Select(a => new
            {
                a.Id,
                Alias = a.AliasAddress,
                a.Status,
                a.CreatedAt,
                a.UserId,
                ForwardingAddresses = a.ForwardingAddresses.Select(f => new
                {
                    f.Id,
                    f.ForwardingAddress
                }).ToList()
            })
            .ToListAsync();
        return Results.Ok(aliases);
    }

    private static async Task<IResult> GetAuthenticatedUserAlias(HttpContext context, int aliasId, EmailAliasDbContext db)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Results.Unauthorized();
        }

        var alias = await db.EmailAliases
            .Include(a => a.ForwardingAddresses)
            .Where(a => a.UserId == userId && a.Id == aliasId)
            .Select(a => new
            {
                a.Id,   
                Alias = a.AliasAddress,
                a.Status,
                a.CreatedAt,
                a.UserId,
                ForwardingAddresses = a.ForwardingAddresses.Select(f => new
                {
                    f.Id,
                    f.ForwardingAddress
                }).ToList()
            })
            .FirstOrDefaultAsync();
        
        return alias == null ? Results.NotFound() : Results.Ok(alias);
    }
} 