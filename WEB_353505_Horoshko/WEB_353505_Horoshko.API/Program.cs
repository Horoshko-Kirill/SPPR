using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WEB_353505_Horoshko.API.Data;
using WEB_353505_Horoshko.API.EndPoints;
using WEB_353505_Horoshko.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем MediatR
builder.Services.AddMediatR(typeof(Program).Assembly);

// DbContext PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// HTTP Context
builder.Services.AddHttpContextAccessor();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Настройки Keycloak
var authServer = builder.Configuration.GetSection("AuthServer").Get<AuthServerData>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var authServer = builder.Configuration.GetSection("AuthServer").Get<AuthServerData>();
        o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known/openid-configuration";
        o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";
        o.RequireHttpsMetadata = false;

        // Игнорируем audience (ключевой момент для client_credentials)
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };

        // Маппинг ролей из realm_access.roles
        o.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                if (ctx.Principal?.Identity is ClaimsIdentity identity)
                {
                    var realmAccess = ctx.Principal.FindFirst("realm_access")?.Value;
                    if (!string.IsNullOrEmpty(realmAccess))
                    {
                        var rolesJson = System.Text.Json.JsonDocument.Parse(realmAccess);
                        if (rolesJson.RootElement.TryGetProperty("roles", out var roles))
                        {
                            foreach (var r in roles.EnumerateArray())
                            {
                                var roleName = r.GetString();
                                if (!string.IsNullOrEmpty(roleName))
                                    identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

// Политики авторизации
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));

});

var app = builder.Build();

// Swagger и OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseStaticFiles(); // уже есть у тебя


app.UseHttpsRedirection();

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapBookEndpoints();

// Seed данных
try
{
    await DbInitializer.SeedData(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
