using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using WEB_353505_Horoshko.HelperClasses;
using WEB_353505_Horoshko.Models;
using WEB_353505_Horoshko.Services.Authentication;
using WEB_353505_Horoshko.Services.Services;

namespace WEB_353505_Horoshko.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ITokenAccessor _tokenAccessor;
        private readonly KeycloakData _options;
        private readonly IFileService _fileService;

        public AccountController(
           IHttpContextAccessor contextAccessor,
           HttpClient httpClient,
           ITokenAccessor tokenAccessor,
           IOptions<KeycloakData> options,
           IFileService fileService)
        {
            _contextAccessor = contextAccessor;
            _httpClient = httpClient;
            _tokenAccessor = tokenAccessor;
            _options = options.Value;
            _fileService = fileService;
        }

        public IActionResult Register()
        {
            return View(new RegisterUserViewModel());
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel user)
        {

            if (!ModelState.IsValid)
                return View(user);

            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, true);
            }
            catch
            {
                return Unauthorized();
            }

            string avatarUrl = "/images/default-profile-picture.png";

            if (user.Avatar != null)
            {
                avatarUrl = await _fileService.SaveFileAsync(user.Avatar);
            }

            var newUser = new CreateUserModel
            {
                Email = user.Email,
                Username = user.Email,
                Enabled = true,
                EmailVerified = true,
                Attributes = new Dictionary<string, string>
                {
                    { "avatar", avatarUrl }
                },
                Credentials = new List<UserCredentials>
                {
                    new UserCredentials { Value = user.Password }
                }
            };

            var requestUri = $"{_options.Host}/admin/realms/{_options.Realm}/users";

            var json = JsonSerializer.Serialize(newUser, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Home");

            return BadRequest(response.StatusCode);
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(
            "keycloak",
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });
        }
        [HttpPost]
        public async Task Logout()
        {
            await
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("keycloak",
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }

    class UserCredentials
    {
        public string Type { get; set; } = "password";
        public bool Temporary { get; set; } = false;
        public string Value { get; set; }
    }

    class CreateUserModel
    {
        public Dictionary<string, string> Attributes { get; set; } = new();
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; } = true;
        public bool EmailVerified { get; set; } = true;
        public List<UserCredentials> Credentials { get; set; } = new();
    }


}
