using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using WEB_353505_Horoshko.Domain.Entities;
using WEB_353505_Horoshko.Services.Authentication;

namespace WEB_353505_Horoshko.UI.Services
{
    public class ApiBookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pageSize;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<ApiBookService> _logger;
        private readonly ITokenAccessor _tokenAccessor;

        public ApiBookService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiBookService> logger, ITokenAccessor tokenAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _tokenAccessor = tokenAccessor;
            _pageSize = configuration.GetValue<string>("ItemsPerPage") ?? "3";
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

                var urlBuilder = new StringBuilder("/api/Book/category");


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

            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

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
        public async Task<ResponseData<Book>> CreateBookAsync(Book book, IFormFile? file)
        {
            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
            }
            catch (Exception e)
            {
                return ResponseData<Book>
                .Error($"Объект не добавлен. Error: {e.Message}");
            }

            using var content = new MultipartFormDataContent();

            book.Image = "Images/noimage.jpg";

            var json = JsonSerializer.Serialize(book, _serializerOptions);
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "bookJson");

            if (file != null && file.Length > 0)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);
            }

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

        public async Task<ResponseData<Book>> UpdateBookAsync(int id, IFormFile? file, Book book)
        {

            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
            }
            catch (Exception e)
            {
                return ResponseData<Book>
                .Error($"Объект не добавлен. Error: {e.Message}");
            }

            using var content = new MultipartFormDataContent();

            var json = JsonSerializer.Serialize(book, _serializerOptions);
            content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "bookJson");

            if (file != null && file.Length > 0)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.FileName);
            }

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

            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

            var response = await _httpClient.DeleteAsync($"/api/Book/{id}");

            if (response.IsSuccessStatusCode)
            {
                return ResponseData<bool>.Success(true);
            }

            return ResponseData<bool>.Error($"Объект не удален. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<List<Book>>> GetAllBookListAsync(string? categoryNormalizedName = null)
        {

            try
            {

                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

                var urlBuilder = new StringBuilder("/api/Book/all");


                if (!string.IsNullOrEmpty(categoryNormalizedName))
                {
                    urlBuilder.Append($"/{categoryNormalizedName}");
                }



                var finalUrl = urlBuilder.ToString();
                _logger.LogInformation($"Запрос к URL: {_httpClient.BaseAddress}{finalUrl}");


                var response = await _httpClient.GetAsync(finalUrl);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content
                            .ReadFromJsonAsync<ResponseData<List<Book>>>(_serializerOptions);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError($"Ошибка JSON: {ex.Message}");
                        return ResponseData<List<Book>>.Error($"Ошибка: {ex.Message}");
                    }
                }

                _logger.LogError($"HTTP ошибка: {response.StatusCode} для URL: {finalUrl}");
                return ResponseData<List<Book>>.Error($"Данные не получены. Error: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Исключение: {ex.Message}");
                return ResponseData<List<Book>>.Error($"Ошибка: {ex.Message}");
            }
        }
    }
}