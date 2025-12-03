using MediatR;

namespace WEB_353505_Horoshko.API.Use_Cases
{

    public sealed record SaveImage(IFormFile File) : IRequest<string>;
    public class SaveImageHandler : IRequestHandler<SaveImage, string>
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SaveImageHandler> _logger;

        public SaveImageHandler(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SaveImageHandler> logger)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> Handle(SaveImage request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}");
            }

            var wwwrootPath = _env.WebRootPath;
            if (string.IsNullOrEmpty(wwwrootPath))
            {
                wwwrootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
            }

            var imagesPath = Path.Combine(wwwrootPath, "Images");
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.File.FileName)}";
            var filePath = Path.Combine(imagesPath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var imageUrl = $"{baseUrl}/Images/{fileName}";

            _logger.LogInformation($"Image saved: {imageUrl}");
            return imageUrl;
        }
    }
}
