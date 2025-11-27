using System.Text;
using System.Text.Json;
using WEB_353505_Horoshko.Domain.Entities;

namespace WEB_353505_Horoshko.UI.Services
{
    public class ApiBookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pageSize;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<ApiBookService> _logger;

        public ApiBookService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiBookService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _pageSize = configuration.GetValue<string>("ItemsPerPage") ?? "3";
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            try
            {

                var urlBuilder = new StringBuilder("/api/Book");


                if (!string.IsNullOrEmpty(categoryNormalizedName))
                {
                    urlBuilder.Append($"/{categoryNormalizedName}");
                }


                var queryParams = new List<string>
                {
                    $"pageNo={pageNo}",
                    $"pageSize={_pageSize}"
                };


                urlBuilder.Append($"?{string.Join("&", queryParams)}");

                var finalUrl = urlBuilder.ToString();
                _logger.LogInformation($"Запрос к URL: {_httpClient.BaseAddress}{finalUrl}");


                var response = await _httpClient.GetAsync(finalUrl);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content
                            .ReadFromJsonAsync<ResponseData<ListModel<Book>>>(_serializerOptions);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"Ошибка JSON: {ex.Message}");
                        return ResponseData<ListModel<Book>>.Error($"Ошибка: {ex.Message}");
                    }
                }

                _logger.LogError($"HTTP ошибка: {response.StatusCode} для URL: {finalUrl}");
                return ResponseData<ListModel<Book>>.Error($"Данные не получены. Error: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Исключение: {ex.Message}");
                return ResponseData<ListModel<Book>>.Error($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ResponseData<Book>> GetBookByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Book/{id}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content
                        .ReadFromJsonAsync<ResponseData<Book>>(_serializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка: {ex.Message}");
                    return ResponseData<Book>.Error($"Ошибка: {ex.Message}");
                }
            }

            return ResponseData<Book>.Error($"Данные не получены. Error: {response.StatusCode}");
        }
        public async Task<ResponseData<Book>> CreateBookAsync(Book book)
        {
            book.Image = "Images/noimage.jpg";

            var json = JsonSerializer.Serialize(book, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Book", content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseData = await response.Content
                        .ReadFromJsonAsync<ResponseData<Book>>(_serializerOptions);
                    return responseData!;
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка: {ex.Message}");
                    return ResponseData<Book>.Error($"Ошибка: {ex.Message}");
                }
            }

            _logger.LogError($"Объект не создан. Error: {response.StatusCode}");
            return ResponseData<Book>.Error($"Объект не добавлен. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Book>> UpdateBookAsync(int id, Book book)
        {
            var json = JsonSerializer.Serialize(book, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/Book/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseData = await response.Content
                        .ReadFromJsonAsync<ResponseData<Book>>(_serializerOptions);
                    return responseData!;
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка: {ex.Message}");
                    return ResponseData<Book>.Error($"Ошибка: {ex.Message}");
                }
            }

            return ResponseData<Book>.Error($"Объект не обновлен. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<bool>> DeleteBookAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Book/{id}");

            if (response.IsSuccessStatusCode)
            {
                return ResponseData<bool>.Success(true);
            }

            return ResponseData<bool>.Error($"Объект не удален. Error: {response.StatusCode}");
        }

        public Task<ResponseData<Book>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, Book product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Book>> CreateProductAsync(Book product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }
    }
}