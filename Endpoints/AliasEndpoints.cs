using EmailAliasApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EmailAliasApi.Endpoints;

public static class AliasEndpoints
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        app.MapGet("/users/{userId}/aliases", GetUserAliases)
            .WithName("GetUserAliases")
            .WithOpenApi();

        app.MapGet("/users/{userId}/aliases/{aliasId}", GetUserAlias)
            .WithName("GetUserAlias")
            .WithOpenApi();
    }

    private static async Task<IResult> GetUserAliases(int userId, EmailAliasDbContext db)
    {
        var aliases = await db.EmailAliases
            .Include(a => a.ForwardingAddresses)
            .Where(a => a.UserId == userId)
            .ToListAsync();
        return Results.Ok(aliases);
    }

    private static async Task<IResult> GetUserAlias(int userId, int aliasId, EmailAliasDbContext db)
    {
        var alias = await db.EmailAliases
            .Include(a => a.ForwardingAddresses)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == aliasId);
        
        return alias == null ? Results.NotFound() : Results.Ok(alias);
    }
} 