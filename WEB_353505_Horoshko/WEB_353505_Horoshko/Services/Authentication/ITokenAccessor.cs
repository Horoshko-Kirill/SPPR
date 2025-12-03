namespace WEB_353505_Horoshko.Services.Authentication
{
    public interface ITokenAccessor
    {
        Task SetAuthorizationHeaderAsync(HttpClient httpClient, bool isClient);
    }
}