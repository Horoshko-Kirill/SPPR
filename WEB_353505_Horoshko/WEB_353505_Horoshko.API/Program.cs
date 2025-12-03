using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WEB_353505_Horoshko.API.Data;
using WEB_353505_Horoshko.API.EndPoints;
using WEB_353505_Horoshko.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(Program).Assembly);

var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var authServer = builder.Configuration.GetSection("AuthServer").Get<AuthServerData>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var authServer = builder.Configuration.GetSection("AuthServer").Get<AuthServerData>();
        o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known/openid-configuration";
        o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";
        o.RequireHttpsMetadata = false;


        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };

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


builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));

});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseStaticFiles(); 


app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.MapBookEndpoints();


try
{
    await DbInitializer.SeedData(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
