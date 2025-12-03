using MediatR;

namespace WEB_353505_Horoshko.API.Use_Cases
{

    public sealed record DeleteImage(string? imageUrl) : IRequest<bool>;
    public class DeleteImageHandler : IRequestHandler<DeleteImage, bool>
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DeleteImageHandler> _logger;

        public DeleteImageHandler(IWebHostEnvironment env, ILogger<DeleteImageHandler> logger)
        {
            _env = env;
            _logger = logger;
        }

        public Task<bool> Handle(DeleteImage request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.imageUrl))
                {
                    return Task.FromResult(true); 
                }

                var fileName = Path.GetFileName(request.imageUrl);
                if (string.IsNullOrEmpty(fileName))
                {
                    return Task.FromResult(true);
                }

                var wwwrootPath = _env.WebRootPath;
                if (string.IsNullOrEmpty(wwwrootPath))
                {
                    wwwrootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
                }
                var filePath = Path.Combine(wwwrootPath, "Images", fileName);


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"Deleted image file: {filePath}");
                    return Task.FromResult(true);
                }
                else
                {
                    _logger.LogWarning($"Image file not found: {filePath}");
                    return Task.FromResult(true); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image: {request.imageUrl}");
                return Task.FromResult(false);
            }
        }
    }
}
