using System.Runtime.CompilerServices;
using WEB_353505_Horoshko.Models;
using WEB_353505_Horoshko.UI.Services;

namespace WEB_353505_Horoshko.Extensions
{
    public static class HostingExtensions
    {
        public static void RegisterCustomServices(this WebApplicationBuilder builder)
        {
            var uriData = builder.Configuration.GetSection("UriData").Get<UriData>() ?? new UriData();

            builder.Services.AddHttpClient<IBookService, ApiBookService>(opt => opt.BaseAddress = new Uri(uriData.ApiUri));

            builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt => opt.BaseAddress = new Uri(uriData.ApiUri));
        }
    }
}
