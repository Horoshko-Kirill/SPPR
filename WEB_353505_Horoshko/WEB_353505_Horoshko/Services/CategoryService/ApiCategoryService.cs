

using System.Net.Http;
using System.Text;
using System.Text.Json;
using WEB_353505_Horoshko.UI.Services;

namespace WEB_353505_Horoshko.Services.CategoryService
{
    public class ApiCategoryService : ICategoryService
    {

        private readonly HttpClient _httpClient;
        private readonly string _pageSize;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<ApiBookService> _logger;

        public ApiCategoryService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiBookService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _pageSize = configuration.GetValue<string>("ItemsPerPage") ?? "3";
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public Task<ResponseData<Category>> CreateCategoryAsync(Category category, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseData<List<Category>>> GetCategoriesAsync()
        {
            try
            {
                var url = "/api/Categories";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return ResponseData<List<Category>>.Error(
                        $"Ошибка при запросе: {response.StatusCode}");
                }

                var data = await response.Content.ReadFromJsonAsync<List<Category>>();

                return ResponseData<List<Category>>.Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Исключение: {ex.Message}");
                return ResponseData<List<Category>>.Error($"Ошибка: {ex.Message}");
            }
        }

        public Task<ResponseData<Category>> GetCategoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategorytAsync(int id, Category category)
        {
            throw new NotImplementedException();
        }
    }
}
