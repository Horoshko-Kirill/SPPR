using WEB_353505_Horoshko.Extensions;
using WEB_353505_Horoshko.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<UriData>(
    builder.Configuration.GetSection("UriData"));


builder.Services.AddControllersWithViews();

builder.RegisterCustomServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
