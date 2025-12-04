using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;
using System.Security.Claims;
using WEB_353505_Horoshko.API.Models;
using WEB_353505_Horoshko.Domain.Help;
using WEB_353505_Horoshko.Extensions;
using WEB_353505_Horoshko.HelperClasses;
using WEB_353505_Horoshko.Models;
using WEB_353505_Horoshko.Services.Authentication;
using WEB_353505_Horoshko.Services.Services;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Загружаем настройки Keycloak
var keycloakData = builder.Configuration
    .GetSection("Keycloak")
    .Get<KeycloakData>();

builder.Services.Configure<KeycloakData>(
    builder.Configuration.GetSection("Keycloak"));

builder.Services.AddControllersWithViews();

builder.RegisterCustomServices();

builder.Services
.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "keycloak";
})
.AddCookie()
.AddOpenIdConnect("keycloak", options =>
{
    options.Authority = $"{keycloakData.Host}/auth/realms/{keycloakData.Realm}";
    options.ClientId = keycloakData.ClientId;
    options.ClientSecret = keycloakData.ClientSecret;
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.Scope.Add("openid"); // Customize scopes as needed
    options.SaveTokens = true;
    options.RequireHttpsMetadata = false; // позволяет обращаться к локальному
    options.MetadataAddress =
    $"{keycloakData.Host}/realms/{keycloakData.Realm}/.well-known/openid-configuration";
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<ITokenAccessor, KeycloakTokenAccessor>();

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeAreaFolder("Admin", "/", "admin"); 
    });

builder.Services.AddScoped<IFileService, LocalFileService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddScoped<Cart, SessionCart>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
);

var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseSession();

app.Use(async (context, next) =>
{
    var roles = string.Join(", ", context.User.Claims
                                         .Where(c => c.Type == ClaimTypes.Role)
                                         .Select(c => c.Value));
    Console.WriteLine($"User roles: {roles}");
    await next();
});



app.Run();
