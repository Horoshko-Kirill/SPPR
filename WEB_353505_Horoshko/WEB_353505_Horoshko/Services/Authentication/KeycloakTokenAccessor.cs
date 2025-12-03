using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using WEB_353505_Horoshko.HelperClasses;
namespace WEB_353505_Horoshko.Services.Authentication

{
    public class KeycloakTokenAccessor : ITokenAccessor
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KeycloakData _options;
        private readonly HttpClient _httpClient;

        public KeycloakTokenAccessor(
            IHttpContextAccessor contextAccessor,
            IOptions<KeycloakData> options,
            HttpClient httpClient)
        {
            _contextAccessor = contextAccessor;
            _options = options.Value;
            _httpClient = httpClient;
        }

        public async Task SetAuthorizationHeaderAsync(HttpClient client, bool isClient)
        {
            string token = isClient
                ? await GetClientToken()
                : await GetUserToken();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetUserToken()
        {
            var context = _contextAccessor.HttpContext;

            var authenticate = await context.AuthenticateAsync("keycloak");
            if (authenticate?.Principal == null)
                throw new Exception("Пользователь не авторизован");

            var token = await context.GetTokenAsync("keycloak", "access_token");
            return token!;
        }

         private async Task<string> GetClientToken()
        {
            string requestUri =
                $"{_options.Host}/realms/{_options.Realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("client_id", _options.ClientId),
                new KeyValuePair<string,string>("grant_type", "client_credentials"),
                new KeyValuePair<string,string>("client_secret", _options.ClientSecret),
            });

            var response = await _httpClient.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ошибка получения client token: " + response.StatusCode);

            var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
            return json!["access_token"]!.ToString();
        }
    }
}
