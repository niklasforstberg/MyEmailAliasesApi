using EmailAliasApi.Data;
using EmailAliasApi.Endpoints;
using EmailAliasApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EmailAliasApi.Models;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers for Cloudflare/Caddy reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto |
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost;
    // Trust all proxies (Cloudflare and Caddy)
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        var originsString = builder.Configuration["ALLOWED_ORIGINS"] ?? "";
        var allowedOrigins = originsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailAlias API", Version = "v1" });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "http",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };

        // Add this section for detailed error messages
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                // Only use this for debugging. Remove in production!
                var result = JsonSerializer.Serialize("401 Error: " + context.Error + " - " + context.ErrorDescription);
                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, User.UserRole.ADMIN.ToString()));
});

// Add DB context
builder.Services.AddDbContext<EmailAliasDbContext>(options =>
{
    var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
    Console.WriteLine("Connection string: " + connectionString);
    options.UseSqlServer(connectionString);
});

// Add Email Service
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Use forwarded headers (must be first)
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        // Detect protocol from forwarded headers (Cloudflare provides HTTPS)
        var scheme = httpReq.Scheme;
        var host = httpReq.Host.Value;
        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{scheme}://{host}" } };
    });
});
app.UseSwaggerUI();

// Enable CORS
app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithTags("Health")
    .AllowAnonymous();

// Map all API endpoints (must be before static files)
app.MapAliasEndpoints();
app.MapAuthEndpoints();

// Serve static files from wwwroot (React app build output)
// Only serve static files if wwwroot exists (for production)
if (Directory.Exists("wwwroot"))
{
    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Fallback to index.html for SPA routing (must be last)
    app.MapFallbackToFile("index.html");
}

// Run database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EmailAliasDbContext>();
    db.Database.Migrate();
}

app.Run();
